using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System.Threading;
using System.Net;

using Model = FeedNews.Models.RSSModel;
using InProgressDialog = FeedNews.Utilities.InProgrressDialog;


namespace FeedNews.ViewModels
{
    

    public class FeedNewsBoardViewModel: ViewModelBase
    {
        private const int MAX_TEXTBLOCK_LENGTH = 250;
        private const int TRANCUATED_LENGTH = 100;
        private string _imagePath;
        private string _itemTitle;
        private string _itemDescr;
        private string _itemMonth;
        private string _itemDay;
        private string _itemTimeSpan;
        private string _currentFeedThumb;
        private string _currentFeedName;
        private string _strTmp;
        private int _userControlHeight;
        private int _userControlWidth;
        private int _headerWidth;
        private int _dockHeight;
        private int _iconWidth;
        private string _isRead;
        private bool updateFlag = false;
        Logger LOG = Logger.getInstance();
        Model obj = Model.getInstance();

        BackgroundWorker worker = new BackgroundWorker();

        private  List<Feeder> _MyData = new List<Feeder>();

        private static FeedNewsBoardViewModel instance = null;
        
        public static FeedNewsBoardViewModel getInstance()
        {
            if (instance == null)
                instance = new FeedNewsBoardViewModel();

            return instance;
        }


        public FeedNewsBoardViewModel()
        {
            Next = new RelayCommand(param => getNextItem());
            Prev = new RelayCommand(param => getPrevItem());
            resize();
            updateAllProperties();
            // Generate button for choosing feeder
            generateButtonFeeder();
        }
        // Properties
        #region Properties
        public string imgPath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
                OnPropertyChanged(new PropertyChangedEventArgs("imgPath"));
            }
        }
        public string itemTitle
        {
            get
            {
                return _itemTitle;
            }
            set
            {
                _itemTitle = value;
                OnPropertyChanged(new PropertyChangedEventArgs("itemTitle"));
            }
        }
        public string itemDescr
        {
            get
            {
                if (_itemDescr.Length > MAX_TEXTBLOCK_LENGTH)
                {
                    _strTmp = _itemDescr.Substring(0, TRANCUATED_LENGTH) + "...";
                    _itemDescr = _strTmp;
                }
                else
                    return _itemDescr;

                return _itemDescr;
            }
            set
            {
                _itemDescr = value;
                OnPropertyChanged(new PropertyChangedEventArgs("itemDescr"));
            }
        }
        public string itemMonth
        {
            get
            {
                return _itemMonth;
            }
            set
            {
                try
                {
                    value = value.Replace("EST", "GMT");
                    DateTime dt = DateTime.Parse(value);
                    _itemMonth = dt.ToString("MMM");
                }
                catch
                {
                    value = value.Replace("ET", "GMT");
                    DateTime dt = DateTime.Parse(value);
                    _itemMonth = dt.ToString("MMM");
                }
                OnPropertyChanged(new PropertyChangedEventArgs("itemMonth"));
            }
        }
        public string itemDay
        {
            get
            {
                return _itemDay;
            }
            set
            {
                try
                {
                    value = value.Replace("EST", "GMT");
                    DateTime dt = DateTime.Parse(value);
                    _itemDay = dt.ToString("dd");
                }
                catch
                {
                    value = value.Replace("ET", "GMT");
                    DateTime dt = DateTime.Parse(value);
                    _itemDay = dt.ToString("dd");
                }
                OnPropertyChanged(new PropertyChangedEventArgs("itemDay"));
            }
        }
        public string itemTimeSpan
        {
            get
            {
                return _itemTimeSpan;
            }
            set
            {
                DateTime updatedTime;
                try
                {
                    value = value.Replace("EST", "GMT");
                     updatedTime = DateTime.Parse(value);
                }
                catch
                {
                    value = value.Replace("ET", "GMT");
                     updatedTime = DateTime.Parse(value);
                }
                
                DateTime currentTime = DateTime.Now;
                TimeSpan elapsedTime = currentTime - updatedTime;

                string day, hour, minutes, seconds;
                day = elapsedTime.Days.ToString();
                hour = elapsedTime.Hours.ToString();
                minutes = elapsedTime.Minutes.ToString();
                seconds = elapsedTime.Seconds.ToString();

                _itemTimeSpan = "Published ";
                if (day != "0")
                {
                    _itemTimeSpan +=( day + " days");
                }
                else if (hour != "0")
                {
                    _itemTimeSpan +=( hour + " hours");
                }
                else if (minutes != "0")
                {
                    _itemTimeSpan += (minutes + " minutes");
                }
                else
                {
                    _itemTimeSpan += (seconds + " seconds");
                }


                _itemTimeSpan += " ago";
                OnPropertyChanged(new PropertyChangedEventArgs("itemTimeSpan"));
            }
        }
        public string CurrentFeedThumb
        {
            get
            {
                return _currentFeedThumb;
            }
            set
            {
                _currentFeedThumb = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentFeedThumb"));
            }
        }
        public string CurrentFeedName
        {
            get
            {
                return _currentFeedName;
            }
            set
            {
                _currentFeedName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentFeedName"));
            }
        }
        public int UserControlHeight
        {
            get
            {
                return _userControlHeight;
                ;
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
        public int HeaderWidth
        {
            get
            {
                return _headerWidth;
                ;
            }
            set
            {
                _headerWidth = value;
                OnPropertyChanged(new PropertyChangedEventArgs("HeaderWidth"));
            }
        }
        public int DockHeight
        {
            get
            {
                return _dockHeight;
            }
            set
            {
                _dockHeight = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DockHeight"));
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
                OnPropertyChanged(new PropertyChangedEventArgs("IconWidth"));
            }

        }
        public string IsReadAlready
        {
            get
            {
                return _isRead;
            }
            set
            {
                if (Model.listXmlContent[Model.gNum].readFlag)
                    //_isRead = "/Icon/isRead.png";
                    _isRead = "0.1";
                else
                    _isRead = "";
                OnPropertyChanged(new PropertyChangedEventArgs("IsReadAlready"));
            } 
        }
        #endregion

        public List<Feeder> MyData 
        { 
            get 
            { 
                return _MyData; 
            }
            set
            {
                _MyData = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MyData"));
            }
        }

        public ICommand Next { get;  set; }
        public ICommand Prev { get; set; }

        // Methods
        public void resize()
        {
            UserControlHeight = (int)MainViewModelLocator._height;
            UserControlWidth = (int)(MainViewModelLocator._width * 0.79);
            HeaderWidth = (int)(MainViewModelLocator._width * 0.8);
            DockHeight = (int)(MainViewModelLocator._height * 0.08);
            IconWidth = (int)(MainViewModelLocator._width * 0.008);
        }
        private void getNextItem()
        {
            Model.gNum += 1;
            if (Model.gNum == Model.gTotalItems)
            {
                Model.gNum = 0;
            }
            /// update bound content
            updateAllProperties();
        }
        private void getPrevItem()
        {
            Model.gNum -= 1;
            if (Model.gNum < 0)
            {
                Model.gNum = Model.gTotalItems - 1;
            }
            /// update bound content
            updateAllProperties();
        }
        public void updateAllProperties()
        {
            if (Model.listXmlContent == null)
            {
                LOG.LoggerPrint("[RSSFeedBoardViewModel::updateAllProperties] listXmlContent is empty.");
                return;
            }
            imgPath = Model.listXmlContent[Model.gNum].imageUri;
            itemTitle = Model.listXmlContent[Model.gNum].title;
            itemDescr = Model.listXmlContent[Model.gNum].description;
            itemMonth = Model.listXmlContent[Model.gNum].pubDate;
            itemDay = Model.listXmlContent[Model.gNum].pubDate;
            itemTimeSpan = Model.listXmlContent[Model.gNum].pubDate;
            CurrentFeedThumb = obj.rssFeedList[Model.gFeederId].Logo;
            CurrentFeedName = obj.rssFeedList[Model.gFeederId].Name;
            IsReadAlready = "";
            //updateGuidList(Model.listXmlContent[Model.gNum].GUID);
        }

        public void generateButtonFeeder()
        {
            updateFlag = false;

            if (Model.listXmlContent == null)
            {
                LOG.LoggerPrint("[RSSFeedBoardViewModel::generateButtonFeeder] listXmlContent is empty.");
                return;
            }

            if (_MyData != null)
            {
                _MyData.Clear();
            }

            for (int i = 0; i < Model.gMaxFeeder; i++)
            {
                if ((obj.rssFeedList[i].Name.Length < 0) || (obj.rssFeedList[i].Index.Length < 0 ))
                    continue;
                if ( obj.rssFeedList[i].Logo.Length <= 0 )
                {
                    LOG.LoggerPrint("[RSSFeedBoardViewModel::generateButtonFeeder] rssFeedList[" + i + "] has no logo");
                    HtmlParser parser = new HtmlParser();
                    parser.getFeederIcon(obj.rssFeedList[i].Uri);
                    obj.rssFeedList[i].Logo = parser.htmlText;
                    LOG.LoggerPrint("[RSSFeedBoardViewModel::generateButtonFeeder] Retrieve log success. logo=" +
                                     obj.rssFeedList[i].Logo);
                    updateFlag = true;
                }
                _MyData.Add(new Feeder(obj.rssFeedList[i].Name,
                        Convert.ToInt32(obj.rssFeedList[i].Index),
                        obj.rssFeedList[i].Logo));
            }
            MyData = _MyData;
            if(updateFlag)
                saveItem(); 
        }
        private void saveItem()
        {
            //Model obj = Model.getInstance();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Model.getFileName()))
            {
                for (int i = 0; i < Model.gMaxFeeder; i++)
                {
                    string line = obj.rssFeedList[i].Index + "," + obj.rssFeedList[i].Name + "," + obj.rssFeedList[i].Uri + "," + obj.rssFeedList[i].Logo;
                    file.WriteLine(line);
                }
            }
            LOG.LoggerPrint("[RSSFeedBoardViewModel::generateButtonFeeder] File saved successfully");
        }
        public void updateGuidList(string guid)
        {
            string guidFile = Model.getFileName("guidId.config");

            ///do nothing if it is read.
            if (Model.listXmlContent[Model.gNum].readFlag)
                return;

            using (System.IO.StreamWriter w = System.IO.File.AppendText(guidFile))
            {
                FeedNews.Models.xmlData item = new FeedNews.Models.xmlData();
                item = Model.listXmlContent[Model.gNum];
                item.readFlag = true;
                Model.listXmlContent.RemoveAt(Model.gNum);
                Model.listXmlContent.Insert(Model.gNum, item);
                w.WriteLine(guid);
            }
        }
    }

    public class Feeder : ViewModelBase
    {
        private string _updateNotiIcon;
        public string Content { get; set; }
        public ICommand Command { get; set; }
        public string Logo { get; set; }
        public string UpdateNotiIcon 
        {
            get
            {
                return _updateNotiIcon;
            }
            set
            {
                _updateNotiIcon = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UpdateNotiIcon"));
            }
        }
        private int index;

        public Feeder(string content, int idx, string logo)
        {
            Content = content;
            index = idx;
            Logo = logo;
            Command = new RelayCommand(param => getFeeder());
        }
        private void getFeeder()
        {
            /// get new xmldata
            /// 
            InProgressDialog.InProgress = new InProgressScreen();
            InProgressDialog.awakeProcess();
            Model.gFeederId = index;
            Model obj = Model.getInstance();
            obj.getXmlData();
            obj.getXmlNodeCnt();

            /// reset item sequence
            Model.gNum = 0;

            /// refresh rendering
            FeedNewsBoardViewModel board = FeedNewsBoardViewModel.getInstance();
            board.updateAllProperties();
            InProgressDialog.abortProcess();
        }

    }
}
