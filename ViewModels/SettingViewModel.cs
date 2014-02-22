using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows;

using RSSFeedList = FeedNews.Models.RSSFeedList;
using Model = FeedNews.Models.RSSModel;
using InProgressDialog = FeedNews.Utilities.InProgrressDialog;

namespace FeedNews.ViewModels
{
    public class SettingViewModel: ViewModelBase
    {
        private ObservableCollection<RSSFeedList> _MyRSSList = new ObservableCollection<RSSFeedList>();
        public ObservableCollection<RSSFeedList> MyRSSList
        {
            get
            {
                return _MyRSSList;
            }
            set
            {
                _MyRSSList = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MyRSSList"));
            }
        }

        Logger LOG = Logger.getInstance();
        Model obj = Model.getInstance();
        public static SettingViewModel _instance;
        public static SettingViewModel getInstance()
        {
            if (_instance == null)
                _instance = new SettingViewModel();

            return _instance;
        }
        public SettingViewModel()
        {
            Model obj = Model.getInstance();

            if (Model.listXmlContent != null)
            {
                for (int cnt = 0; cnt < Model.gMaxFeeder; cnt++)
                {
                    _MyRSSList.Add(new RSSFeedList(obj.rssFeedList[cnt].Index,
                                                    obj.rssFeedList[cnt].Name,
                                                    obj.rssFeedList[cnt].Uri,
                                                    obj.rssFeedList[cnt].Logo));
                }
            }
            itemUrl = "http://";
            AddItem = new RelayCommand(param => addItem(itemName, itemUrl,""), param => ruleCheck());
            DeleteItem = new RelayCommand(param => deleteItem(_thisItem), param =>  thisItem != null);
            SaveItem = new RelayCommand(param => saveItem());
            _viewHeight = MainViewModelLocator._height;
            _viewWidth = MainViewModelLocator._width;
            //resize();
        }
        public ICommand AddItem { get; set; }
        public ICommand DeleteItem { get; set; }
        public ICommand SaveItem { get; set; }
        public void addItem(string name, string url, string logo)
        {
            InProgressDialog.InProgress = new InProgressScreen();
            InProgressDialog.awakeProcess();
            
            _MyRSSList.Add(new RSSFeedList((Model.gMaxFeeder++).ToString(), name, url, logo));
            MyRSSList = _MyRSSList;

            /// append to file
            using (System.IO.StreamWriter w = System.IO.File.AppendText(Model.getFileName()))
            {
                // _MyRSSList array index start from 0
                string line = _MyRSSList[Model.gMaxFeeder-1].Index + "," + _MyRSSList[Model.gMaxFeeder-1].Name + "," + _MyRSSList[Model.gMaxFeeder-1].Uri + "," + _MyRSSList[Model.gMaxFeeder-1].Logo;
                w.WriteLine(line);
            }

            if (Model.listXmlContent == null)
            {
                obj.getRssList();
                obj.getXmlData();
                obj.getXmlNodeCnt();
            }
            refrestFeederButton();
            itemName = "";
            itemUrl = "";
            InProgressDialog.abortProcess();
        }
        private void deleteItem(RSSFeedList item)
        {
            int idx;
            idx = Convert.ToInt32(item.Index);
            rearrangeIndex(idx);
            _MyRSSList.Remove(item);
            --Model.gMaxFeeder;
            MyRSSList = _MyRSSList;
            saveItem();
        }
        private void saveItem()
        {
            InProgressDialog.InProgress = new InProgressScreen();
            InProgressDialog.awakeProcess();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Model.getFileName()))
            {
                for(int i = 0; i< Model.gMaxFeeder; i++)
                {
                    string line = _MyRSSList[i].Index + "," + _MyRSSList[i].Name + "," + _MyRSSList[i].Uri + "," + _MyRSSList[i].Logo;
                    file.WriteLine(line);
                }
            }
            refrestFeederButton();
            InProgressDialog.abortProcess();
        }
        private void rearrangeIndex(int index)
        {
            for (int i = index++; i < Model.gMaxFeeder; i++)
            {
                _MyRSSList[i].Index = (i - 1).ToString();
            }
            MyRSSList = _MyRSSList;
        }
        private void refrestFeederButton()
        {
            FeedNewsBoardViewModel boardViewer = FeedNewsBoardViewModel.getInstance();
            Model obj = Model.getInstance();

            obj.getRssList();
            boardViewer.generateButtonFeeder();
        }
        private bool ruleCheck()
        {
            if (itemName == null || itemUrl == null)
                return false;
            else
            {
                if ((itemName.Length > 0) || itemUrl.Length > 0)
                    return true;
            }
            return false;
        }
        private string _itemName;
        private string _itemUrl;
        public string itemName
        {
            get
            {
                return _itemName;
            }
            set
            {
                _itemName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("itemName"));
            }
        }
        public string itemUrl
        {
            get
            {
                return _itemUrl;
            }
            set
            {
                _itemUrl = value;
                OnPropertyChanged(new PropertyChangedEventArgs("itemUrl"));
            }
        }

        private RSSFeedList _thisItem;
        public RSSFeedList thisItem
        {
            get
            {
                return _thisItem;
            }
            set
            {
                _thisItem = value;
                OnPropertyChanged(new PropertyChangedEventArgs("thisItem"));
            }
        }
        List<String> _comboItem = new List<string>();
        private void initComboItem()
        {
            _comboItem.Add("640x480");
            _comboItem.Add("800x600");
            _comboItem.Add("1024x768");
            _comboItem.Add("1280x768");
        }
        public List<string> ComboItem
        {
            get
            {
                initComboItem();
                return _comboItem;
            }
        }
        private string _thisComboItem;
        public string ThisComboItem
        {
            get
            {
                return _thisComboItem;
            }
            set
            {
                _thisComboItem = value;
                resizeAll(_thisComboItem);
                OnPropertyChanged(new PropertyChangedEventArgs("ThisComboItem"));
            }
        }
        private void resizeAll(string item)
        {
            MainViewModelLocator locator = MainViewModelLocator.getInstance();
            string[] words = item.Split('x');
            MainViewModelLocator._width = Convert.ToInt32(words[0]);
            MainViewModelLocator._height = Convert.ToInt32(words[1]);
            LOG.LoggerPrint("[DEBUG]: width:" + MainViewModelLocator._width + " height:" + MainViewModelLocator._height);
            locator.resize();
        }
        private int _viewHeight, _viewWidth;
        public int ViewHeight
        {
            get
            {
                return _viewHeight;
            }
            set
            {
                _viewHeight = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ViewHeight"));
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
                OnPropertyChanged(new PropertyChangedEventArgs("ViewWidth"));
            }
        }
        private int _textBoxWidth;
        public int TextBoxWidth
        {
            get
            {
                return _textBoxWidth;
            }
            set
            {
                _textBoxWidth = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TextBoxWidth"));
            }
        }
        public void resize()
        {
            TextBoxWidth = (int)(MainViewModelLocator._width * 0.6);
            LOG.LoggerPrint("DEBUG:MainViewModelLocator._width?" + MainViewModelLocator._width);
        }
    }
}
