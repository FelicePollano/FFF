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

        static void Main(string[] args)
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
            
            tasks.Add(Task.Run(() => {

                Explore(Directory.GetCurrentDirectory(), tasks,args[0],args[1]);
            })); //dir explorer
            while (tasks.Any(t => !t.IsCompleted))
            {
                await Task.WhenAll(tasks.ToArray());
            }
            }
            );
        }

        private static void Explore(string dir, ConcurrentBag<Task> tasks,string filespec,string tosearch)
        {
            var subs = Directory.GetDirectories(dir);
            foreach (var sub in subs)
                tasks.Add(Task.Run(()=>Explore(sub,tasks,filespec,tosearch)));
            foreach (var fl in Directory.GetFiles(dir,filespec))
            {
                tasks.Add(Task.Run(()=>Process(fl,tosearch)));
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
