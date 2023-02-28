using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FFFui
{
    public class RunCompareCommand : ICommand
    {
        private readonly CompareSourceViewModel model;
        private readonly ViewModel mainViewModel;

        public RunCompareCommand(CompareSourceViewModel model,ViewModel mainViewModel)
        {
            this.model = model;
            this.mainViewModel = mainViewModel;
        }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return !string.IsNullOrEmpty(model.FileName1) && !string.IsNullOrEmpty(model.FileName2);
        }

        public void Execute(object? parameter)
        {
            try
            {
                Environment.SetEnvironmentVariable("file1", model.FileName1);
                Environment.SetEnvironmentVariable("file2", model.FileName2);
                Process process = new Process();
                var cmdLine = Settings.Instance.GetCommandForCompare();
                ProcessStartInfo startInfo = new ProcessStartInfo(
                        Environment.ExpandEnvironmentVariables(cmdLine.Item1),
                        Environment.ExpandEnvironmentVariables(cmdLine.Item2));
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception ex)
            {
                mainViewModel.HasErrors = true;
                mainViewModel.ErrorMessage = ex.ToString();
            }
        }

        internal void FireChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public class CompareSourceViewModel
    {
        public RunCompareCommand RunCompare { get; set; }
        public CompareSourceViewModel(ViewModel mainViewModel)
        {
            RunCompare = new RunCompareCommand(this,mainViewModel);
        }
        private string fileName1;
        private string fileName2;

        public string FileName1 { get => fileName1; set { fileName1 = value; RunCompare.FireChanged(); } }
        public string FileName2 { get => fileName2; set { fileName2 = value; RunCompare.FireChanged(); } }
    }
}
