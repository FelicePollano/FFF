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

    internal class ViewModel : INotifyPropertyChanged
    {
        class SearchCommand : ICommand
        {
            ViewModel model;
            public SearchCommand(ViewModel model)
            {
                this.model = model;
                model.PropertyChanged += (s, e) =>
                  {
                      if (e.PropertyName == nameof(ViewModel.ToSearch))
                      {
                          CanExecuteChanged?.Invoke(this, new EventArgs());
                      }
                  };
            }

            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return !string.IsNullOrEmpty(model.ToSearch);
            }

            public void Execute(object? parameter)
            {
                model.Tabs.Add(new SearchViewModel(model.ToSearch));
                model.Selected = model.Tabs.Count - 1;
            }


        }

        private string toSearch;
        private int selected;

        public int Selected
        {
            get { return selected; }
            set 
            { 
                selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
            }
        }

        public string ToSearch
        {
            get { return toSearch; }
            set {
                toSearch = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ToSearch)));
            }
        }


        public ICommand Search { get; private set; }

        public ViewModel()
        {
            Search = new SearchCommand(this);
            tabs = new ObservableCollection<SearchViewModel>();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private bool useRegex;
        private ObservableCollection<SearchViewModel> tabs;

        public ObservableCollection<SearchViewModel>  Tabs
        {
            get { return tabs; }
            set { tabs = value; }
        }


        public bool UseRegex
        {
            get { 
                return useRegex;
            }
            set {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseRegex)));
                useRegex = value; 
            }
        }

    }
}
