
using SimpleConfigParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FFFui
{
    internal class Settings
    {
        Dictionary<string, Tuple<string, string>> extensionMapFile = new Dictionary<string, Tuple<string, string>>();
        Dictionary<string, Tuple<string, string>> extensionMapAtLine = new Dictionary<string, Tuple<string, string>>();
        Tuple<string, string> compareCmd = new Tuple<string, string>(null, null);
        static Settings instance;
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
            var cfgPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ".fffui/config");
            Configuration config = new Configuration();
            using(var sr = new StreamReader(cfgPath))
            {
                config.Add(sr.ReadToEnd());
            }

            if (config.Sections.ContainsKey("compare"))
            {
                compareCmd = new Tuple<string, string>(config.Sections["compare"]["cmd"], config.Sections["compare"]["cmdline"]);
            }

            if (config.Sections.ContainsKey("extension"))
            {
                if (config.Sections["extension"].ContainsKey("open-cmd"))
                {
                    var cmdline = "";
                    if (config.Sections["extension"].ContainsKey("open-cmdline"))
                    {
                        cmdline = config.Sections["extension"]["open-cmdline"];
                    }
                    extensionMapFile["<default>"] = new Tuple<string,string>(config.Sections["extension"]["open-cmd"],cmdline);
                }
                if (config.Sections["extension"].ContainsKey("open-atline-cmd"))
                {
                    var cmdline = "";
                    if (config.Sections["extension"].ContainsKey("open-atline-cmdline"))
                    {
                        cmdline = config.Sections["extension"]["open-atline-cmdline"];
                    }
                    extensionMapAtLine["<default>"] = new Tuple<string, string>(config.Sections["extension"]["open-atline-cmd"], cmdline);
                }
                foreach (var sub in config.Sections["extension"].Sections.Keys)
                {
                    foreach (var ext in sub.Split(','))
                    {
                        if (config.Sections["extension"].Sections[sub].ContainsKey("open-cmd"))
                        {
                            var cmdline = "";
                            if (config.Sections["extension"].Sections[sub].ContainsKey("open-cmdline"))
                            {
                                cmdline = config.Sections["extension"].Sections[sub]["open-cmdline"];
                            }
                            extensionMapFile[ext] = new Tuple<string, string>(config.Sections["extension"].Sections[sub]["open-cmd"], cmdline);
                        }
                        if (config.Sections["extension"].Sections[sub].ContainsKey("open-atline-cmd"))
                        {
                            var cmdline = "";
                            if (config.Sections["extension"].Sections[sub].ContainsKey("open-atline-cmdline"))
                            {
                                cmdline = config.Sections["extension"].Sections[sub]["open-atline-cmdline"];
                            }
                            extensionMapAtLine[ext] = new Tuple<string, string>(config.Sections["extension"].Sections[sub]["open-atline-cmd"], cmdline);
                        }
                    }
                }
            }
        }
        public Tuple<string, string> GetCommandForCompare()
        {
            return compareCmd;
        }
        public Tuple<string,string> GetCommandLineForOpeningAtLine(string extensionWithoutDot)
        {
            return GetCmd(extensionWithoutDot, extensionMapAtLine);
        }

        private static Tuple<string, string> GetCmd(string extensionWithoutDot, Dictionary<string, Tuple<string, string>> dict)
        {
            if (dict.ContainsKey(extensionWithoutDot))
                return dict[extensionWithoutDot];
            if (dict.ContainsKey("<default>"))
                return dict["<default>"];
            throw new Exception($"Can't handle extension .{extensionWithoutDot}");
        }

       
        public Tuple<string,string> GetCommandLineForOpening(string extensionWithoutDot)
        {
            return GetCmd(extensionWithoutDot, extensionMapAtLine);
        }
    }
}
