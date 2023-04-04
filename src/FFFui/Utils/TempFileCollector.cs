using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFFui.Utils
{
    internal class TempFileCollector
    {
        string collectListPath;
        static TempFileCollector()
        {
            Instance = new TempFileCollector();
            
        }
        private TempFileCollector()
        {
            collectListPath = Path.Combine(Path.GetTempPath(), ".fffremove.me");
        }
        public static TempFileCollector Instance { get; private set; }

        public void AddFileToCollect(string file)
        {
            using (var sw=new StreamWriter(collectListPath,true))
            {
                sw.WriteLine(file);
            }
        }
        public void Collect()
        {
            bool noError = true;
            using (var sr = new StreamReader(collectListPath))
            {
                string fileName;
                while (null != (fileName = sr.ReadLine()))
                {
                    if (File.Exists(fileName))
                    {
                        try
                        {
                            File.Delete(fileName);
                        }
                        catch
                        {
                            noError = false;
                        }
                    }
                }
            }
            if (noError)
            {
                File.Delete(collectListPath);
            }
        }

    }
}
