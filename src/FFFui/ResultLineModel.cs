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
    public class OpenAtLineCommand : ICommand
    {
        private readonly ResultLineModel model;

        public OpenAtLineCommand(ResultLineModel model)
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
            Environment.SetEnvironmentVariable("file", parameter as string);
            Environment.SetEnvironmentVariable("line", model.LineNumber.ToString());
            Process process = new Process();
            var cmdLine = Settings.Instance.GetCommandLineForOpeningAtLine(Path.GetExtension(parameter as string).Trim('.'));
            ProcessStartInfo startInfo = new ProcessStartInfo(
                    Environment.ExpandEnvironmentVariables(cmdLine.Item1),
                    Environment.ExpandEnvironmentVariables(cmdLine.Item2));
            process.StartInfo = startInfo;
            process.Start();
        }
    }
    public class ResultLineModel
    {
        public ResultLineModel()
        {
            OpenAtLine = new OpenAtLineCommand(this);
        }
        public OpenAtLineCommand OpenAtLine { get; private set; }
        public string Line { get; set; }
        public int LineNumber { get; set; }
        
    }
}
