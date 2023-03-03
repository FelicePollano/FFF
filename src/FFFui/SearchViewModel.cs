using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FFFui
{
    
    class CloseTabCommand : ICommand
    {
        private readonly ViewModel model;
        private readonly SearchViewModel searchViewModel;

        public CloseTabCommand(ViewModel model,SearchViewModel searchViewModel)
        {
            this.model = model;
            this.searchViewModel = searchViewModel;
        }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            model.Tabs.Remove(searchViewModel);
            RemoveFromCompareSource();
        }

        private void RemoveFromCompareSource()
        {
            foreach (var one in searchViewModel.Results)
            {
                if (one.FileName == model.CompareSourceViewModel.FileName1)
                {
                    model.CompareSourceViewModel.FileName1 = null;
                    model.UpdateAllCompare();
                }
                else if (one.FileName == model.CompareSourceViewModel.FileName2)
                {
                    model.CompareSourceViewModel.FileName2 = null;
                    model.UpdateAllCompare();
                }
            }
        }
    }
    public class SearchViewModel : INotifyPropertyChanged
    {
        public bool HasRepoResults => RepoHistory.Count > 0;
        public IList<RepoHistoryLineViewModel> RepoHistory { get; private set; }
        private string historyOfFile;

        public string HistoryOfFile
        {
            get { return historyOfFile; }
            set { historyOfFile = value; PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(HistoryOfFile))); }
        }

        public ICommand CloseTab { get; private set; }
       
        public string ToSearch { get; private set; }
        public SearchViewModel(string toSearch, ViewModel mainViewModel)
        {
            ToSearch= toSearch;
            this.mainViewModel = mainViewModel;
            Results = new ObservableCollection<ResultModel>();
            CloseTab = new CloseTabCommand(mainViewModel,this);
            
            this.RepoHistory = new ObservableCollection<RepoHistoryLineViewModel>();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private ObservableCollection<ResultModel> results;
        private readonly ViewModel mainViewModel;
        
        public ObservableCollection<ResultModel> Results
        {
            get { return results; }
            set { results = value; }
        }

        internal void UpdateHistoryResult()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasRepoResults)));
        }
    }
}
