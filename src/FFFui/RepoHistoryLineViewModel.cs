using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFFui
{
    public class RepoHistoryLineViewModel
    {
        public string Revision { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public string Comments { get; set; }
    }
}
