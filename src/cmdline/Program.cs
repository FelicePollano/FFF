using Fff.Crawler;
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
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace fff
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand()
            {
               new Option<string>(new []{"--path","-p"},getDefaultValue:()=>Directory.GetCurrentDirectory(),"path where to search")
                ,new Option<string[]>(new []{"-f","--files"},"use double quotes to avoid wildcard expansion, ie \"*.cpp\" [default: *.*]")
                ,new Option<bool>(new[]{"-i","--ignore-case"},getDefaultValue:()=>false,"ignore case")
                ,new Option<bool>(new[]{"-x","--use-regex"},getDefaultValue:()=>false,"use search string as a pattern")
                ,new Option<bool>(new[]{"-n","--name-only"},getDefaultValue:()=>false,"just look for file names")
                ,new Option<bool>(new[]{"-j","--json"},getDefaultValue:()=>false,"output in json without coloring")
                ,new Argument<string>("search","string to search for")
                
            };
            
            rootCommand.Handler= CommandHandler.Create<string,string,string[],bool,bool,bool,bool>(async (search,path,files,ignoreCase,useRegex,nameonly,json)=> {
                
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                
                Regex regex = null;
                Crawler crawler;
                if(useRegex)
                {
                    try{
                        regex = new Regex(search,ignoreCase?RegexOptions.Compiled|RegexOptions.IgnoreCase:RegexOptions.Compiled);
                        crawler = new Crawler(files.Length==0?new []{"*.*"}:files,regex,nameonly);
                    }
                    catch(Exception e)
                    {
                        Console.Error.WriteLine($"Cannot parse {search} as a regular expression:{e.Message}");
                        return;
                    }
                }
                else
                {
                    crawler = new Crawler(files.Length==0?new []{"*.*"}:files,search,ignoreCase,nameonly);
                }
                crawler.Report = (results,file)=>{
                    if(!json){
                        Console.Error.WriteLine($"\t- {file}".Pastel(Color.BlueViolet));
                        foreach (var r in results)
                        {
                            if(r.LineNumber>0)
                            {
                                Console.Error.WriteLine($" {(r.LineNumber.ToString() + ": ").PastelBg(Color.White).Pastel(Color.Black)} {r.Line}");
                            }
                        }
                    }
                    else{
                        Console.Out.WriteLine(JsonSerializer.Serialize(new{file=file,findings=results}));
                    }
                };
                await crawler.Crawl(path);
                Console.Error.WriteLine($"processed {crawler.Count.ToString().Pastel(Color.CadetBlue)} files in {stopWatch.Elapsed.ToString().Pastel(Color.Chocolate)} seconds.");
            });
            rootCommand.Description="Fast Search in Files";
            await rootCommand.InvokeAsync(args);
           
        }
    }
}
