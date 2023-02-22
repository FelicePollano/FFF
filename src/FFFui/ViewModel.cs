using Fff.Crawler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Threading;
using System.Threading;
using AutoMapper;

namespace FFFui
{

    internal class ViewModel : INotifyPropertyChanged
    {
        class ChangePathCommand : ICommand
        {
            ViewModel model;
            public ChangePathCommand(ViewModel model)
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
                

                var dlg = new CommonOpenFileDialog();
                dlg.Title = "Select Folder";
                dlg.IsFolderPicker = true;
                dlg.InitialDirectory = model.Path;

                dlg.AddToMostRecentlyUsedList = false;
                dlg.AllowNonFileSystemItems = false;
                dlg.DefaultDirectory = model.Path;
                dlg.EnsureFileExists = true;
                dlg.EnsurePathExists = true;
                dlg.EnsureReadOnly = false;
                dlg.EnsureValidNames = true;
                dlg.Multiselect = false;
                dlg.ShowPlacesList = true;

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                   model.Path = dlg.FileName;
                }
            }
        }
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

            public async void Execute(object? parameter)
            {
                var viewModel = new SearchViewModel(model.ToSearch);
                model.Tabs.Add(viewModel);
                model.Selected = model.Tabs.Count - 1;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                Regex regex;
                Crawler crawler;
                var files = string.IsNullOrEmpty(model.Types)?new string[0]: model.Types.Split(';');
                if (model.UseRegex)
                {
                    try
                    {
                        regex = new Regex(model.ToSearch ,!model.MatchCase?RegexOptions.Compiled | RegexOptions.IgnoreCase : RegexOptions.Compiled);
                        crawler = new Crawler(files.Length == 0 ? new[] { "*.*" } : files, regex, model.NameOnly);
                    }
                    catch (Exception e)
                    {
                        throw;
                        //Console.Error.WriteLine($"Cannot parse {model.ToSearch} as a regular expression:{e.Message}");
                        return;
                    }
                }
                else
                {
                    crawler = new Crawler(files.Length == 0 ? new[] { "*.*" } : files, model.ToSearch, !model.MatchCase, model.NameOnly);
                }
                var uiContext = SynchronizationContext.Current;
                crawler.Report = (results, file) => {

                    uiContext.Send(x => viewModel.Results.Add(new ResultModel(model.CompareSourceViewModel) { Results = TheMapper.Mapper.Map<IList<Result>,IList<ResultLineModel>>(results), FileName = file }), null);
                    
                };
                
                await crawler.Crawl(model.Path);

            }


        }

        private string toSearch;
        private int selected;
        private string types;
        private string path;


        public CompareSourceViewModel CompareSourceViewModel { get; private set; }

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
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path)));
            }
        }
        public string Types
        {
            get { return types; }
            set
            {
                types = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Types)));
            }
        }

        public ICommand Search { get; private set; }
        public ICommand ChangePath { get; private set; }

        public ViewModel()
        {
            Path = Directory.GetCurrentDirectory();
            Search = new SearchCommand(this);
            ChangePath = new ChangePathCommand(this);
            tabs = new ObservableCollection<SearchViewModel>();
            CompareSourceViewModel = new CompareSourceViewModel();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private bool useRegex;
        private bool matchCase;
        private bool nameOnly;
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
        public bool MatchCase
        {
            get
            {
                return matchCase;
            }
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MatchCase)));
                matchCase = value;
            }
        }
        public bool NameOnly
        {
            get
            {
                return nameOnly;
            }
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NameOnly)));
                nameOnly = value;
            }
        }

    }
}
