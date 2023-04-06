using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public static void  DumpAtRevision(string filename, string destPath, string rev)
        {
            var folder = Path.GetDirectoryName(filename);
            string root;
            var t = GetRepositoryType(folder, out root);
            if (t == RepoType.Git)
            {
                new GitRepository(root).DumpAtRevision(filename, destPath, rev);
            }
            else if (t == RepoType.Hg)
            {
                new HgRepository(root).DumpAtRevision(filename, destPath, rev);
            }
            else
            {
                throw new Exception("Unsupported repository type, or no repository");
            }
        }
        public static void GetHistory(string filename, Action<HistoryEntry> collectEntry)
        {
            var folder = Path.GetDirectoryName(filename);
            string root;
            var t = GetRepositoryType(folder,out root);
            if (t == RepoType.Git)
            {
                 new GitRepository(root).GetHistory(filename,collectEntry);
            }
            else if (t == RepoType.Hg)
            {
                new HgRepository(root).GetHistory(filename,collectEntry);
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

        

        static public RepoType GetRepositoryType(string folder,out string root)
        {
            if (Directory.Exists(Path.Combine(folder, ".hg")))
            {
                root = folder;
                return RepoType.Hg;
            }
            else if (Directory.Exists(Path.Combine(folder, ".git")))
            {
                root = folder;
                return RepoType.Git;
            }
            else {
                if (Directory.GetParent(folder) != null)
                {
                    return GetRepositoryType(Directory.GetParent(folder).FullName,out root);
                }
                else
                {
                    root = null;
                    return RepoType.None;
                }
             }
        }
    }
}
