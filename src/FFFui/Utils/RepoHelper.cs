using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFFui.Utils
{
    // git log --pretty=format:"%h|%an|%ad|%s" --date=iso --

    public class RepoHelper
    {
        public enum RepoType
        {
            Hg,Git,None
        }
        public class HistoryEntry
        {
            public string Revision { get; set; }
            public DateTime Date { get; set; }
            public string User { get; set; }
            public string Comments { get; set; }
        }
        public static void GetHistory(string filename, Action<HistoryEntry> collectEntry)
        {
            var folder = Path.GetDirectoryName(filename);
            var t = GetRepositoryType(folder);
            if (t == RepoType.Git)
            {
                 new GitRepository().GetHistory(filename,collectEntry);
            }
            else if (t == RepoType.Hg)
            {
                new HgRepository().GetHistory(filename,collectEntry);
            }
            else
            {
                throw new Exception("Unsupported repository type, or no repository");
            }
            

        }

        private static IList<HistoryEntry> GetHistoryHg(string filename, Action<HistoryEntry> collectEntry)
        {
            throw new NotImplementedException();
        }

        

        static public RepoType GetRepositoryType(string folder)
        {
            if (Directory.Exists(Path.Combine(folder, ".hg")))
            {
                return RepoType.Hg;
            }
            else if (Directory.Exists(Path.Combine(folder, ".git")))
            {
                return RepoType.Git;
            }
            else {
                if (Directory.GetParent(folder) != null)
                {
                    return GetRepositoryType(Directory.GetParent(folder).FullName);
                }
                else
                {
                    return RepoType.None;
                }
             }
        }
    }
}
