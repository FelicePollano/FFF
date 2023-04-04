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
        private readonly string root;

        public GitRepository(string root)
        {
            this.root = root;
        }
        public void DumpAtRevision(string filename, string destPath,string rev)
        {
            var wdir = Path.GetDirectoryName(filename);
            var opt = new FileStreamOptions() { Access= FileAccess.ReadWrite, Mode=FileMode.Create};
            using (var outputStream = new StreamWriter(destPath, new UTF8Encoding(false),opt))
            {

                var process = new System.Diagnostics.Process();
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "git.exe",
                    Arguments = String.Format($"show {rev}:\"{Path.GetRelativePath(root, filename).Replace("\\", "/")}\""),
                    UseShellExecute = false,
                    //CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = root,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };


                process.EnableRaisingEvents = true;
                process.Start();
                process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (e.Data != null)
                    {
                        outputStream.WriteLine(e.Data);
                    }
                });
                process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (e.Data != null)
                    {
                        outputStream.WriteLine(e.Data);
                    }
                });
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit();
                outputStream.Flush();
                TempFileCollector.Instance.AddFileToCollect(destPath);
            }
           
        }

        public void GetHistory(string filename, Action<RepoHelper.HistoryEntry> collectEntry)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "git.exe",
                Arguments = String.Format($"log --pretty=format:\" % h |% an |% ad |% s\" --date=iso -- \"{filename}\""),
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
