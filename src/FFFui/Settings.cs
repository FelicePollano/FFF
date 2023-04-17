
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FFFui
{
    public class Editor
    {
        public string[] Ext { get; set; }
        public string OpenCommand { get; set; }
        public string CmdLine { get; set; }
        public string CmdLineAt { get; set; }
    }
    public class CompareTool
    {
        public string OpenCommand { get; set; }
        public string CmdLine { get; set; }
    }
    internal class YamlSettings
    {
        public Editor[] Editors {get;set;}
        public Editor DefaultEditor { get; set; }
        public CompareTool Compare { get; set; }
    }
    internal class Settings
    {
        static Settings instance;
        YamlSettings configuration;
        public static Settings Instance
        {
            get
            {
                lock (typeof(Settings))
                {
                    if (null == instance)
                        instance = new Settings();
                    return instance;
                }
            }
        }
        private Settings()
        {
            var cfgPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ".fffui/config.yaml");
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.HyphenatedNamingConvention.Instance)
                .Build();
            using (var sr1 = new StreamReader(cfgPath))
            {
                configuration = deserializer.Deserialize<YamlSettings>(sr1);
            }
        }
        public Tuple<string, string> GetCommandForCompare()
        {
            return new Tuple<string,string>(configuration.Compare.OpenCommand,configuration.Compare.CmdLine);
        }
        public Tuple<string,string> GetCommandLineForOpeningAtLine(string extensionWithoutDot)
        {
            foreach (var editor in configuration.Editors)
            {
                if (editor.Ext.Contains(extensionWithoutDot))
                    return new Tuple<string, string>(editor.OpenCommand, editor.CmdLineAt);
            }
            return new Tuple<string, string>(configuration.DefaultEditor.OpenCommand, configuration.DefaultEditor.CmdLineAt);
        }
       
        public Tuple<string,string> GetCommandLineForOpening(string extensionWithoutDot)
        {
            foreach (var editor in configuration.Editors)
            {
                if (editor.Ext.Contains(extensionWithoutDot))
                    return new Tuple<string, string>(editor.OpenCommand, editor.CmdLine);
            }
            return new Tuple<string, string>(configuration.DefaultEditor.OpenCommand, configuration.DefaultEditor.CmdLine);
        }
    }
}
