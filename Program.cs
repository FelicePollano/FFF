using CommandLine;
using Pastel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        static async Task Main(string tosearch,string path=".",string[] files=null)
        {
            var parser = new Parser(with =>
            {
                with.EnableDashDash = true;
                with.HelpWriter = Console.Error;
                
            }
            );
            
            var options = parser.ParseArguments<Options>(args);

            options.WithParsed(async opt =>  {
            
            
            
            ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();
            if(files==null)
                files=new string[]{"*.*"};
            tasks.Add(Task.Run(() => {

                Explore(path, tasks,files,tosearch);
            })); //dir explorer
            while (tasks.Any(t => !t.IsCompleted))
            {
                await Task.WhenAll(tasks.ToArray());
            }
            }
            );
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
