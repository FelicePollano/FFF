using System;
using System.Collections.Generic;
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
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
