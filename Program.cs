using Pastel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fff
{
    class Program
    {
        static int count = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tosearch">string to search for</param>
        /// <param name="path">path where to search for the string</param>
        /// <param name="files">fiels wildcard to search</param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand()
            {
               new Option<string>(new []{"--path","-p"},getDefaultValue:()=>Directory.GetCurrentDirectory(),"path where to search")
                ,new Option<string>(new []{"-f","--files"},getDefaultValue:()=>"*.*")
                ,new Argument<string>("search","string to search for")
            };
            
            rootCommand.Handler= CommandHandler.Create<string,string,string>(async (search,path,files)=> {
            
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();
               
                tasks.Add(Task.Run(() => {

                    Explore(path, tasks,new string[]{files},search);
                })); //dir explorer
                while (tasks.Any(t => !t.IsCompleted))
                {
                    await Task.WhenAll(tasks.ToArray());
                }
                Console.Error.WriteLine($"processed {count.ToString().Pastel(Color.CadetBlue)} files in {stopWatch.Elapsed.ToString().Pastel(Color.Chocolate)} seconds.");
            });
            rootCommand.Description="Fast Search in Files";
            await rootCommand.InvokeAsync(args);
           
        }
        private static void Explore(string dir, ConcurrentBag<Task> tasks,string[] filespec,string tosearch)
        {
            var subs = Directory.GetDirectories(dir);
            foreach (var sub in subs)
                tasks.Add(Task.Run(()=>Explore(sub,tasks,filespec,tosearch)));
            foreach(string fc in filespec)
            {
                foreach (var fl in Directory.GetFiles(dir,fc))
                {
                    tasks.Add(Task.Run(()=>Process(fl,tosearch)));
                }
            }
        }

        private static async Task Process(string fl,string tosearch)
        {
            Interlocked.Increment(ref count);
            int linecount = 0;
            List<Result> report = new List<Result>();
            using(var sr = new StreamReader(fl))
            {
                string line;
                while (null != (line = await sr.ReadLineAsync()))
                {
                    if (-1 != line.IndexOf(tosearch))
                    {
                        linecount++;
                        report.Add(new Result() { Line=line,LineNumber=linecount });
                       
                    }
                }
            }
            lock ( typeof(Console)) //avoid overlapping in result reporting...
            {
                if(report.Count>0)
                    Console.Error.WriteLine($"\t- {fl}".Pastel(Color.BlueViolet));
                foreach (var r in report)
                {
                    Console.Error.WriteLine($" {(r.LineNumber.ToString() + ": ").PastelBg(Color.White).Pastel(Color.Black)} {r.Line}");
                }
            }
        }
    }
}
