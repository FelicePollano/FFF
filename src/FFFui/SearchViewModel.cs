using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFFui
{
    internal class SearchViewModel : INotifyPropertyChanged
    {
        public string ToSearch { get; private set; }
        public SearchViewModel(string toSearch)
        {
            ToSearch= toSearch;
            Results = new ObservableCollection<ResultModel>();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private ObservableCollection<ResultModel> results;

        public ObservableCollection<ResultModel> Results
        {
            get { return results; }
            set { results = value; }
        }
    }
}
