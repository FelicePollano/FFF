using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

namespace Fff.Crawler
{
    
    public sealed class Crawler
    {
        private readonly string[] filespec;
        private readonly string tosearch;
        private readonly bool ignoreCase;
        private readonly Regex regex;
        private readonly bool nameonly;
        private int count;

        public int Count { get{ return count;}  }
        ConcurrentBag<Task> tasks;
        Action<IList<Result>,string> report;
        public Crawler(string[] filespec,Regex regex, bool nameonly)
            :this(filespec,null,false,regex,nameonly)
        {

        }
        public Crawler(string[] filespec,string tosearch,bool ignoreCase, bool nameonly)
            :this(filespec,tosearch,ignoreCase,null,nameonly)
        {

        }

        private Crawler(string[] filespec,string tosearch,bool ignoreCase,Regex regex, bool nameonly)
        {
            this.filespec = filespec;
            this.tosearch = tosearch;
            this.ignoreCase = ignoreCase;
            this.regex = regex;
            this.nameonly = nameonly;
            this.tasks = new ConcurrentBag<Task>();
        }


        public async Task Crawl(string path)
        {
            tasks.Add(Task.Run(() => {
                    Explore(path);
                })); //dir explorer
                while (tasks.Any(t => !t.IsCompleted))
                {
                    await Task.WhenAll(tasks.ToArray());
                }
        }
        public Action<IList<Result>,string> Report { get => report; set => report = value; }

        private  void Explore(string dir)
        {
            try
            {
                var subs = Directory.GetDirectories(dir);
                foreach (var sub in subs)
                    tasks.Add(Task.Run(() => Explore(sub)));
                foreach (string fc in filespec)
                {
                    foreach (var fl in Directory.GetFiles(dir, fc))
                    {
                        tasks.Add(Task.Run(() => Process(fl)));
                    }
                }
            }
            catch (UnauthorizedAccessException )
            {
                //swallow these exceptions, behave as directory does not exist
            }
        }

        private  bool IsInString(string line,string search,bool ignoreCase,Regex regex)
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
        private  async Task Process(string fl)
        {
            Interlocked.Increment(ref count);
            int linecount = 0;
            List<Result> results = new List<Result>();
            if(!nameonly)
            {
                using(var sr = new StreamReader(fl))
                {
                    string line;
                    while (null != (line = await sr.ReadLineAsync()))
                    {
                        linecount++;
                        if (IsInString(line,tosearch,ignoreCase,regex))
                        {
                            results.Add(new Result() { Line=line,LineNumber=linecount });
                        }
                    }
                }
            }
            else
            {
                if(IsInString(Path.GetFileName(fl),tosearch,ignoreCase,regex))
                {
                     results.Add(new Result() { Line=null,LineNumber=0 });
                }
            }
            lock ( typeof(Crawler)) //avoid overlapping in result reporting...
            {
               
                if(results.Count>0 && null != Report)
                {
                    Report(results,fl);
                }
            }
        }
    }
}
