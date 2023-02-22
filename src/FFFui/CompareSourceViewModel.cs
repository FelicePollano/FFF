using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FFFui
{
    internal class RunCompareCommand : ICommand
    {
        private readonly CompareSourceViewModel model;

        public RunCompareCommand(CompareSourceViewModel model)
        {
            this.model = model;
        }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return !string.IsNullOrEmpty(model.FileName1) && !string.IsNullOrEmpty(model.FileName2);
        }

        public void Execute(object? parameter)
        {
            Environment.SetEnvironmentVariable("file1",model.FileName1);
            Environment.SetEnvironmentVariable("file2", model.FileName2);
            Process process = new Process();
            var cmdLine = Settings.Instance.GetCommandForCompare();
            ProcessStartInfo startInfo = new ProcessStartInfo(
                    Environment.ExpandEnvironmentVariables(cmdLine.Item1),
                    Environment.ExpandEnvironmentVariables(cmdLine.Item2));
            process.StartInfo = startInfo;
            process.Start();
        }

        internal void FireChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    internal class CompareSourceViewModel
    {
        public RunCompareCommand RunCompare { get; set; }
        public CompareSourceViewModel()
        {
            RunCompare = new RunCompareCommand(this);
        }
        private string fileName1;
        private string fileName2;

        public string FileName1 { get => fileName1; set { fileName1 = value; RunCompare.FireChanged(); } }
        public string FileName2 { get => fileName2; set { fileName2 = value; RunCompare.FireChanged(); } }
    }
}
