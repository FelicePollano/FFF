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
    internal class GitRepository : IRepository
    {
        public void GetHistory(string filename, Action<RepoHelper.HistoryEntry> collectEntry)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "git.exe",
                Arguments = String.Format($"log --pretty=format:\" % h |% an |% ad |% s\" --date=iso -- \"{filename}\""),
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(filename)

            };
            process.Start();
            StreamReader reader = process.StandardOutput;
            string line;
            while (null != (line = reader.ReadLine()))
            {
                var tokens = line.Split('|');
                collectEntry(new RepoHelper.HistoryEntry()
                {
                    Revision = tokens[0],
                    User = tokens[1],
                    Date = DateTime.Parse(tokens[2], CultureInfo.InvariantCulture),
                    Comments = String.Join('|', tokens[3..])

                });
            }
        }
    }
}
