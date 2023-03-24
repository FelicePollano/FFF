using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFFui.Utils
{
    internal class HgRepository : IRepository
    {
        private readonly string root;

        public HgRepository(string root)
        {
            this.root = root;
        }
        public void DumpAtRevision(string filename, string destPath,string rev)
        {
            throw new NotImplementedException();
        }

        public void GetHistory(string filename, Action<RepoHelper.HistoryEntry> collectEntry)
        {
            //hg log --follow --template "{node}|{user}|{date|isodate}|{desc}\n"
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "hg.exe",
                Arguments = $"log --follow --template \"{{node}}|{{user}}|{{date|isodate}}|{{desc}}\\n\" \"{Path.GetFileName(filename)}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
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
                    Revision = tokens[0].Trim(),
                    User = tokens[1],
                    Date = DateTime.Parse(tokens[2], CultureInfo.InvariantCulture),
                    Comments = String.Join('|', tokens[3..])

                });
            }
        }
    }
}
