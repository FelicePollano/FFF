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
        static public IList<HistoryEntry> GetHistory(string filename)
        {
            var folder = Path.GetDirectoryName(filename);
            var t = GetRepositoryType(folder);
            if (t == RepoType.Git)
            {
                return GetHistoryGit(filename);
            }
            else if (t == RepoType.Hg)
            {
                return GetHistoryHg(filename);
            }
            else
            {
                throw new Exception("Unsupported repository type, or no repository");
            }
            

        }

        private static IList<HistoryEntry> GetHistoryHg(string filename)
        {
            throw new NotImplementedException();
        }

        private static IList<HistoryEntry> GetHistoryGit(string filename)
        {
            List<HistoryEntry> history = new List<HistoryEntry>();
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "git.exe",
                Arguments = String.Format($"log --pretty=format:\" % h |% an |% ad |% s\" --date=iso -- \"{filename}\""),
                UseShellExecute = false,
                WindowStyle=ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(filename)

            };
            process.Start();
            StreamReader reader = process.StandardOutput;
            string line;
            while (null != (line = reader.ReadLine()))
            {
                var tokens = line.Split('|');
                history.Add(new HistoryEntry() {
                    Revision=tokens[0],
                    User=tokens[1],
                    Date=DateTime.Parse(tokens[2],CultureInfo.InvariantCulture),
                    Comments=String.Join('|',tokens[3..])
                
                });
            }
            return history;
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
