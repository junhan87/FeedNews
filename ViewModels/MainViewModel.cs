using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace FeedNews.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// The current view.
        /// </summary>
        private ViewModelBase _currentViewModel;

        /// <summary>
        /// Static instance of one of the ViewModels.
        /// </summary>
        readonly static SettingViewModel _secondViewModel = SettingViewModel.getInstance();


        /// <summary>
        /// Static instance of one of the ViewModels.
        /// </summary>
        readonly static FeedNewsViewModel _firstViewModel = new FeedNewsViewModel();

        readonly static SearchRSSViewModel _thirdViewModel = SearchRSSViewModel.getInstance();

        /// <summary>
        /// The CurrentView property.  The setter is private since only this 
        /// class can change the view via a command.  If the View is changed,
        /// we need to raise a property changed event (via INPC).
        /// </summary>
        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                if (_currentViewModel == value)
                    return;
                _currentViewModel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentViewModel"));
            }
        }

        /// <summary>
        /// Simple property to hold the 'FirstViewCommand' - when executed
        /// it will change the current view to the 'FirstView'
        /// </summary>
        public ICommand ViewRSSFeedCommand { get; private set; }

        /// <summary>
        /// Simple property to hold the 'SecondViewCommand' - when executed
        /// it will change the current view to the 'SecondView'
        /// </summary>
        public ICommand SettingViewCommand { get; private set; }


        public ICommand SearchRSSViewCommand { get; private set; }


        /// <summary>
        /// Default constructor.  We set the initial view-model to 'FirstViewModel'.
        /// We also associate the commands with their execution actions.
        /// </summary>
        public MainViewModel()
        {
            CurrentViewModel = MainViewModel._firstViewModel;
            ViewRSSFeedCommand = new RelayCommand(param => ExecuteViewRSSFeedCommand());
            SettingViewCommand = new RelayCommand(param => ExecuteSettingViewCommand());
            SearchRSSViewCommand = new RelayCommand(param => ExecuteSearchRSSViewCommand());
        }

        /// <summary>
        /// Set the CurrentViewModel to 'FirstViewModel'
        /// </summary>
        private void ExecuteViewRSSFeedCommand()
        {
            CurrentViewModel = MainViewModel._firstViewModel;
        }

        /// <summary>
        /// Set the CurrentViewModel to 'SecondViewModel'
        /// </summary>
        private void ExecuteSettingViewCommand()
        {
            CurrentViewModel = MainViewModel._secondViewModel;
        }

        private void ExecuteSearchRSSViewCommand()
        {
            CurrentViewModel = MainViewModel._thirdViewModel;
        }
    }
}
