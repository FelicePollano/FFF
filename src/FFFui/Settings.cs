using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFFui
{
    internal class Settings
    {

        static Settings instance;
        private Settings()
        {

        }
        public static Settings Instance {
            get 
            {
                lock (typeof(Settings))
                {
                    if( null == instance )
                        instance = new Settings();
                    return instance;
                }
            }
        }
        public Tuple<string,string> GetCommandLineForOpening(string extensionWithoutDot)
        {
            return new Tuple<string,string>(@"%programfiles%\Microsoft Visual Studio\2022\Professional\Common7\IDE\devenv.exe", "/edit \"%file%\"");
        }
    }
}
