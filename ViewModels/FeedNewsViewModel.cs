using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;

using InProgressDialog = FeedNews.Utilities.InProgrressDialog;
using Model = FeedNews.Models.RSSModel;

namespace FeedNews.ViewModels
{
    public class FeedNewsViewModel : ViewModelBase
    {
        private ViewModelBase _currentView;
        private string _commandName;
        public static int _userControlHeight;
        public static int _userControlWidth;

        readonly static FeedNewsBoardViewModel _rssFeedBoard = FeedNewsBoardViewModel.getInstance();
        readonly static FeedNewsHtmlViewModel _rssFeedHtml = new FeedNewsHtmlViewModel();

        public FeedNewsViewModel()
        {
            CommandName = "More...";
            resize();
            currentUserControl = FeedNewsViewModel._rssFeedBoard;
            ExecuteInteraction = new RelayCommand(param => doExecuteInteraction());


            // check updated post every 60 secs
            Utilities.UpdateChecker updateChecker = new Utilities.UpdateChecker();
            Thread updateThread = new Thread(updateChecker.startAuditCheckUpdate);
            updateThread.IsBackground = true;
            updateThread.Start();
        }
        public ViewModelBase currentUserControl
        {
            get
            {
                return _currentView;
            }
            set
            {
                _currentView = value;
                OnPropertyChanged(new PropertyChangedEventArgs("currentUserControl"));
            }
        }

        public string CommandName
        {
            get
            {
                return _commandName;
            }
            set
            {
                _commandName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CommandName"));
            }
        }

        public ICommand ExecuteInteraction { get; private set; }
        private void doExecuteInteraction()
        {
            if (currentUserControl == FeedNewsViewModel._rssFeedBoard)
            {
                CommandName = "Back";
                // fetch html text
                InProgressDialog.InProgress = new InProgressScreen();
                InProgressDialog.awakeProcess();
                _rssFeedHtml.getHtmlContent();
                InProgressDialog.abortProcess();
                currentUserControl = FeedNewsViewModel._rssFeedHtml;
                _rssFeedBoard.updateGuidList(Model.listXmlContent[Model.gNum].GUID);
                _rssFeedBoard.IsReadAlready = "";
            }
            else
            {
                CommandName = "More...";
                currentUserControl = FeedNewsViewModel._rssFeedBoard;
            }
        }
        public int UserControlHeight
        {
            get
            {
                return _userControlHeight;
            }
            set
            {
                _userControlHeight = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UserControlHeight"));
            }
        }
        public int UserControlWidth
        {
            get
            {
                return _userControlWidth;
                ;
            }
            set
            {
                _userControlWidth = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UserControlWidth"));
            }
        }
        private void resize()
        {
            UserControlHeight = MainViewModelLocator._height;
            UserControlWidth = (int)(MainViewModelLocator._width * 0.79);
        }
    }
}
