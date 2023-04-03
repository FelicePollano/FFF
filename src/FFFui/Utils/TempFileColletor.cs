using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFFui.Utils
{
    internal class TempFileColletor
    {
        string collectListPath;
        static TempFileColletor()
        {
            Instance = new TempFileColletor();
            
        }
        private TempFileColletor()
        {
            collectListPath = Path.Combine(Path.GetTempPath(), ".fffremove.me");
        }
        public static TempFileColletor Instance { get; private set; }

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
