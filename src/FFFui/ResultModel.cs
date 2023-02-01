using Fff.Crawler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FFFui
{
    class OpenFileCommand : ICommand
    {
        ResultModel model;
        public OpenFileCommand(ResultModel model)
        {
            this.model = model;
        }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Environment.SetEnvironmentVariable("file", model.FileName);
            Environment.SetEnvironmentVariable("line", "1");
            Process process = new Process();
            var cmdLine = Settings.Instance.GetCommandLineForOpening(Path.GetExtension(model.FileName).Trim('.'));
            ProcessStartInfo startInfo = new ProcessStartInfo(
                    Environment.ExpandEnvironmentVariables(cmdLine.Item1),
                    Environment.ExpandEnvironmentVariables(cmdLine.Item2));
            process.StartInfo = startInfo;
            process.Start();
        }
    }

    internal class ResultModel
    {
        public OpenFileCommand OpenFile { get; private set; }
        public ResultModel()
        {
            Results=new List<ResultLineModel>();
            FileName = String.Empty;
            OpenFile = new OpenFileCommand(this);
        }
        public IList<ResultLineModel> Results { get; set; }
        public string FileName { get; set; }
    }
}
