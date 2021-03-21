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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace fff
{
    class Program
    {
        static int count = 0;
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand()
            {
               new Option<string>(new []{"--path","-p"},getDefaultValue:()=>Directory.GetCurrentDirectory(),"path where to search")
                ,new Option<string[]>(new []{"-f","--files"},"use double quotes to avoid wildcard expansion, ie \"*.cpp\" [default: *.*]")
                ,new Option<bool>(new[]{"-i","--ignore-case"},getDefaultValue:()=>false,"ignore case")
                ,new Option<bool>(new[]{"-x","--use-regex"},getDefaultValue:()=>false,"use search string as a pattern")
                ,new Argument<string>("search","string to search for")
                
            };
            
            rootCommand.Handler= CommandHandler.Create<string,string,string[],bool,bool>(async (search,path,files,ignoreCase,useRegex)=> {
                
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();
                Regex regex = null;
                if(useRegex)
                {
                    try{
                        regex = new Regex(search,ignoreCase?RegexOptions.Compiled|RegexOptions.IgnoreCase:RegexOptions.Compiled);
                    }
                    catch(Exception e)
                    {
                        Console.Error.WriteLine($"Cannot parse {search} as a regular expression:{e}");
                    }
                }
               
                tasks.Add(Task.Run(() => {

                    Explore(path, tasks,files.Length==0?new []{"*.*"}:files,search,ignoreCase,regex);
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
        
        private static void Explore(string dir, ConcurrentBag<Task> tasks,string[] filespec,string tosearch,bool ignoreCase,Regex regex)
        {
            var subs = Directory.GetDirectories(dir);
            foreach (var sub in subs)
                tasks.Add(Task.Run(()=>Explore(sub,tasks,filespec,tosearch,ignoreCase,regex)));
            foreach(string fc in filespec)
            {
                foreach (var fl in Directory.GetFiles(dir,fc))
                {
                    tasks.Add(Task.Run(()=>Process(fl,tosearch,ignoreCase,regex)));
                }
            }
        }

        private static bool IsInString(string line,string search,bool ignoreCase,Regex regex)
        {
            if(regex == null)
            {
                return -1 != line.IndexOf(search,ignoreCase?StringComparison.OrdinalIgnoreCase:StringComparison.Ordinal);
            }
            else
            {
                return regex.IsMatch(line);
            }
        }
        private static async Task Process(string fl,string tosearch,bool ignoreCase,Regex regex)
        {
            Interlocked.Increment(ref count);
            int linecount = 0;
            List<Result> report = new List<Result>();
            using(var sr = new StreamReader(fl))
            {
                string line;
                while (null != (line = await sr.ReadLineAsync()))
                {
                    linecount++;
                    if (IsInString(line,tosearch,ignoreCase,regex))
                    {
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
