using FFFui.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FFFui
{
    class OpenAtRevisionCommand : ICommand
    {
        private readonly RepoHistoryLineViewModel model;

        public OpenAtRevisionCommand(RepoHistoryLineViewModel model)
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
            var filename = parameter.ToString();
            model.EnsureShadowFile(filename);
            
            ResultModel dummy = new ResultModel(null,null,null);
            dummy.FileName = model.ShadowFile;
            var cmd = new OpenFileCommand(dummy);
            cmd.Execute(null);

        }
    }


    public class RepoHistoryLineViewModel:IHasCompareSource,INotifyPropertyChanged
    {
        private  CompareSourceViewModel csvm;
        private  ViewModel mainViewModel;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand OpenAtRevision { get; private set; }
        public ICommand AddToCompare { get; private set; }
        public RepoHistoryLineViewModel()
        {
            OpenAtRevision = new OpenAtRevisionCommand(this);
            AddToCompare = new AddToCompareCommand(this);
           
        }
        public string Revision { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public string Comments { get; set; }
        public string ShadowFile { get; set; }

        public CompareSourceViewModel CompareSourceViewModel { get => csvm; set => csvm = value; }

        public ViewModel MainViewModel { get => mainViewModel; set => mainViewModel = value; }

        public string FileName { 
            get => ShadowFile;
        }
        public bool IsCompareSource { get => (CompareSourceViewModel.FileName1 == FileName || CompareSourceViewModel.FileName2 == FileName) && !string.IsNullOrEmpty(FileName); }
        public int CompareSourceOrdinal { get => CompareSourceViewModel.FileName1 == FileName ? 1 : (CompareSourceViewModel.FileName2 == FileName ? 2 : 0); }
        public string OriginalFile { get;  set; }

        public void FireCompareChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompareSource)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CompareSourceOrdinal)));
        }

        public void EnsureShadowFile(string filename)
        {
            if (string.IsNullOrEmpty(ShadowFile))
            {
                var destFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                ShadowFile = Path.ChangeExtension(destFile, Path.GetExtension(filename));
                RepoHelper.DumpAtRevision(filename, ShadowFile, Revision);
            }
        }

        public void BeforeAddToCompare()
        {
            if (null == FileName)
            {
                EnsureShadowFile(OriginalFile);
            }
        }
    }
}
