using Fff.Crawler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FFFui
{
    class CopyLinkCommand : ICommand
    {
        ResultModel model;
        public CopyLinkCommand(ResultModel model)
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
            Clipboard.SetText(model.FileName);
        }
    }
    class OpenInExplorerCommand : ICommand
    {
        ResultModel model;
        public OpenInExplorerCommand(ResultModel model)
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
            System.Diagnostics.Process.Start("explorer.exe",Path.GetDirectoryName(model.FileName));
        }
    }
    class OpenPromptHereCommand : ICommand
    {
        ResultModel model;
        public OpenPromptHereCommand(ResultModel model)
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
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                WorkingDirectory = Path.GetDirectoryName(model.FileName),
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal,
                FileName = "cmd.exe",
               
            };
            process.Start();
        }
    }
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
        public CopyLinkCommand CopyLink { get; private set; }
        public OpenInExplorerCommand OpenInExplorer { get; private set; }
        public OpenPromptHereCommand OpenPromptHere { get; private set; }
        public ResultModel()
        {
            Results=new List<ResultLineModel>();
            FileName = String.Empty;
            OpenFile = new OpenFileCommand(this);
            CopyLink = new CopyLinkCommand(this);
            OpenInExplorer = new OpenInExplorerCommand(this);
            OpenPromptHere = new OpenPromptHereCommand(this);
        }
        public IList<ResultLineModel> Results { get; set; }
        public string FileName { get; set; }
    }
}
