using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeedNews.ViewModels
{
    public class MainViewModelLocator: ViewModelBase
    {
        private static MainViewModel _main;
        public static int _height;
        public static int _width;
        private int _columnWidth;
        private int _iconWidth;
        private int _viewWidth;
        private static MainViewModelLocator _instance;
        Logger LOG = Logger.getInstance();

        public static MainViewModelLocator getInstance()
        {
            if (_instance == null)
            {
                _instance = new MainViewModelLocator();

            }
            return _instance;
        }
        public MainViewModelLocator()
        {
            _instance = this;
            _main = new MainViewModel();
            resize();
        }

        public MainViewModel Main
        {
            get
            {
                return _main;
            }
        }
        public int mainWindowHeight
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("maxWindowHeight"));
            }
        }
        public int mainWindowWidth
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("mainWindowWidth"));
            }
        }
        public int mainMaxHeigth
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("maxMainHeight"));
            }
        }
        public int mainMaxWidth
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("mainMaxWidth"));
            }
        }
        public int ColumnWidth
        {
            get
            {
                return _columnWidth;
            }
            set
            {
                _columnWidth = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ColumnWidth"));
            }
        }
        public int ViewWidth
        {
            get
            {
                return _viewWidth;
            }
            set
            {
                _viewWidth = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ViewWidth"));
            }
        }
        public int IconWidth
        {
            get
            {
                return _iconWidth;
            }
            set
            {
                _iconWidth = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IconWidth"));
            }
        }
        public void resize()
        {
            //mainWindowHeight = 768;
            //mainWindowWidth = 1024;
            mainWindowHeight = (int)(System.Windows.SystemParameters.PrimaryScreenHeight * 0.9);
            mainWindowWidth = (int)(System.Windows.SystemParameters.PrimaryScreenWidth * 0.6);
            ColumnWidth = (int)(mainWindowWidth * 0.15);
            ViewWidth = mainWindowWidth - ColumnWidth;
            IconWidth = (int)(ColumnWidth - (ColumnWidth * 0.2));
            FeedNewsBoardViewModel BoardVM = FeedNewsBoardViewModel.getInstance();
            SearchRSSViewModel SearchRSSVM = SearchRSSViewModel.getInstance();
            BoardVM.resize();
            SearchRSSVM.resize();
            LOG.LoggerPrint("[MainViewModelLocator resize]: windowHeight:" + mainWindowHeight + " windowWidth:" + mainWindowWidth);
        }
    }

    public class RSSFeedModelLocator
    {
        private static FeedNewsViewModel _main;

        public RSSFeedModelLocator()
        {
            _main = new FeedNewsViewModel();
        }

        public FeedNewsViewModel Main
        {
            get
            {
                return _main;
            }
        }
    }
}
