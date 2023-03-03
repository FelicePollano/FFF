using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FFFui.Utils
{
    public interface IRepository
    {
        void GetHistory(string filename, Action<RepoHelper.HistoryEntry> collectEntry);
        void DumpAtRevision(string filename, string destPath, string rev);
    }
}
