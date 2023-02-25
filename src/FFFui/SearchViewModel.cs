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
        }
    }
    public class SearchViewModel : INotifyPropertyChanged
    {
        public ICommand CloseTab { get; private set; }
        public string ToSearch { get; private set; }
        public SearchViewModel(string toSearch, ViewModel mainViewModel)
        {
            ToSearch= toSearch;
            this.mainViewModel = mainViewModel;
            Results = new ObservableCollection<ResultModel>();
            CloseTab = new CloseTabCommand(mainViewModel,this);
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private ObservableCollection<ResultModel> results;
        private readonly ViewModel mainViewModel;

        public ObservableCollection<ResultModel> Results
        {
            get { return results; }
            set { results = value; }
        }
    }
}
