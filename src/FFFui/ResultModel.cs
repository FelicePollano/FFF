using Fff.Crawler;
using FFFui.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FFFui
{
    public class CopyLinkCommand : ICommand
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
            try
            {
                Clipboard.SetText(model.FileName);
            }
            catch (Exception ex)
            {
                model.MainViewModel.HasErrors = true;
                model.MainViewModel.ErrorMessage = ex.Message;
            }
        }
    }
    public class OpenInExplorerCommand : ICommand
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
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(model.FileName));
            }
            catch (Exception ex)
            {
                model.MainViewModel.HasErrors = true;
                model.MainViewModel.ErrorMessage = ex.Message;
            }
}
    }
    public class OpenPromptHereCommand : ICommand
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
            try
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
            catch (Exception ex)
            {
                model.MainViewModel.HasErrors = true;
                model.MainViewModel.ErrorMessage = ex.Message;
            }



        }
    }
    public class AddToCompareCommand:ICommand
    {
        private readonly ResultModel model;

        public AddToCompareCommand(ResultModel model)
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
            try
            {
                if (model.FileName == model.CompareSourceViewModel.FileName1 ||
                    model.FileName == model.CompareSourceViewModel.FileName2
                    )
                {
                    //toggle
                    if (model.CompareSourceViewModel.FileName1 == model.FileName)
                    {
                        model.CompareSourceViewModel.FileName1 = null;
                        model.FireCompareChanged();
                        return;
                    }

                    if (model.CompareSourceViewModel.FileName2 == model.FileName)
                    {
                        model.CompareSourceViewModel.FileName2 = null;
                        model.FireCompareChanged();
                        return;
                    }
                }
                if (model.CompareSourceViewModel.FileName1 == null)
                {
                    model.CompareSourceViewModel.FileName1 = model.FileName;
                    model.FireCompareChanged();
                    return;
                }

                if (model.CompareSourceViewModel.FileName2 == null)
                {
                    model.CompareSourceViewModel.FileName2 = model.FileName;
                    model.FireCompareChanged();
                    return;
                }
            }
            catch (Exception ex)
            {
                model.MainViewModel.HasErrors = true;
                model.MainViewModel.ErrorMessage = ex.Message;
            }

        }
    }
    public class OpenFileCommand : ICommand
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
            try
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
            catch (Exception ex)
            {
                model.MainViewModel.HasErrors = true;
                model.MainViewModel.ErrorMessage = ex.Message;
            }
        }
    }

    public class OpenHistoryCommand : ICommand
    {
        ResultModel model;
        public OpenHistoryCommand(ResultModel model)
        {
            this.model = model;
        }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return RepoHelper.GetRepositoryType(Path.GetDirectoryName(model.FileName)) != RepoHelper.RepoType.None;
        }

        public void Execute(object? parameter)
        {
            try
            {
                RepoHelper.GetHistory(model.FileName);
            }
            catch (Exception ex)
            {
                model.MainViewModel.HasErrors = true;
                model.MainViewModel.ErrorMessage = ex.Message;
            }
        }
    }

    public class ResultModel:INotifyPropertyChanged
    {
        private readonly CompareSourceViewModel csvm;
        private readonly ViewModel mainViewModel;

        public event PropertyChangedEventHandler? PropertyChanged;

        public OpenFileCommand OpenFile { get; private set; }
        public CopyLinkCommand CopyLink { get; private set; }
        public OpenInExplorerCommand OpenInExplorer { get; private set; }
        public OpenPromptHereCommand OpenPromptHere { get; private set; }
        public AddToCompareCommand AddToCompare { get; private set; }
        public OpenHistoryCommand OpenHistory { get; private set; }
        public ResultModel(CompareSourceViewModel csvm,ViewModel mainViewModel)
        {
            Results=new List<ResultLineModel>();
            FileName = String.Empty;
            OpenFile = new OpenFileCommand(this);
            CopyLink = new CopyLinkCommand(this);
            OpenInExplorer = new OpenInExplorerCommand(this);
            OpenPromptHere = new OpenPromptHereCommand(this);
            AddToCompare = new AddToCompareCommand(this);
            OpenHistory = new OpenHistoryCommand(this);
            this.csvm = csvm;
            this.mainViewModel = mainViewModel;
        }
        public IList<ResultLineModel> Results { get; set; }
        public string FileName { get; set; }
        public bool IsCompareSource { get => CompareSourceViewModel.FileName1 == FileName || CompareSourceViewModel.FileName2 == FileName; }
        public int CompareSourceOrdinal { get => CompareSourceViewModel.FileName1 == FileName ? 1: (CompareSourceViewModel.FileName2==FileName ? 2:0 );  }

        public CompareSourceViewModel CompareSourceViewModel => csvm;

        public ViewModel MainViewModel => mainViewModel;

        public void FireCompareChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompareSource)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CompareSourceOrdinal)));
        }
    }
}
