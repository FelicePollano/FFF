using Fff.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFFui
{
    internal class ResultModel
    {
        public ResultModel()
        {
            Results=new List<Result>();
            FileName = String.Empty;
        }
        public IList<Result> Results { get; set; }
        public string FileName { get; set; }
    }
}
