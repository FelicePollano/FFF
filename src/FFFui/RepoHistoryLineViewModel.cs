using FFFui.Utils;
using System;
using System.Collections.Generic;
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
            if (string.IsNullOrEmpty(model.ShadowFile))
            {
                var destFile = Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString());
                model.ShadowFile = Path.ChangeExtension(destFile, Path.GetExtension(filename));
                RepoHelper.DumpAtRevision(filename, model.ShadowFile, model.Revision);
            }
            ResultModel dummy = new ResultModel(null,null,null);
            dummy.FileName = model.ShadowFile;
            var cmd = new OpenFileCommand(dummy);
            cmd.Execute(null);

        }
    }


    public class RepoHistoryLineViewModel
    {
        public ICommand OpenAtRevision { get; private set; }
        public RepoHistoryLineViewModel()
        {
            OpenAtRevision = new OpenAtRevisionCommand(this);
        }
        public string Revision { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public string Comments { get; set; }
        public string ShadowFile { get; set; }
    }
}
