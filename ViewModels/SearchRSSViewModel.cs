using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Windows.Input;
using System.Collections.ObjectModel;

using RSSFeedList = FeedNews.Models.RSSFeedList;

namespace FeedNews.ViewModels
{
    public struct CategoryList
    {
        public string category;
        public string xmlUrl;
    };
    public class FeedList : ViewModelBase
    {
        private string name, description, uri, logo;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            }
        }
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Description"));
            }
        }
        public string Uri
        {
            get
            {
                return uri;
            }
            set
            {
                uri = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Uri"));
            }
        }
        public string Logo
        {
            get
            {
                return logo;
            }
            set
            {
                logo = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Logo"));
            }
        }
        public ICommand AddItem { get; set; }
        
        private int _categoryListEntryHeight, _categoryListEntrylogoWidth, _categoryListEntryNameWidth, _categoryListEntryButtonWidth;
        public int CategoryListEntryHeight
        {
            get
            {
                return _categoryListEntryHeight;
            }
            set
            {
                _categoryListEntryHeight = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CategoryListEntryHeight"));
            }
        }
        public int CategoryListEntrylogoWidth
        {
            get
            {
                return _categoryListEntrylogoWidth;
            }
            set
            {
                _categoryListEntrylogoWidth = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CategoryListEntrylogoWidth"));
            }
        }
        public int CategoryListEntryNameWidth
        {
            get
            {
                return _categoryListEntryNameWidth;
            }
            set
            {
                _categoryListEntryNameWidth = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CategoryListEntryNameWidth"));
            }
        }
        public int CategoryListEntryButtonWidth
        {
            get
            {
                return _categoryListEntryButtonWidth;
            }
            set
            {
                _categoryListEntryButtonWidth = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CategoryListEntryButtonWidth"));
            }
        }
        public FeedList(string name, string descrp, string url, string logo)
        {
            Name = name;
            Description = descrp;
            Uri=url;
            Logo = logo;
            AddItem = new RelayCommand(param => addItem());
            CategoryListEntryHeight = (int)(MainViewModelLocator._height * 0.06);
            CategoryListEntrylogoWidth = (int)(MainViewModelLocator._width * 0.05);
            CategoryListEntryNameWidth = (int)(MainViewModelLocator._width * 0.42);
            CategoryListEntryButtonWidth = (int)(MainViewModelLocator._width * 0.05);
        }
        private void addItem()
        {
            SettingViewModel settingVM = SettingViewModel.getInstance();
            settingVM.addItem(Name,Uri,Logo);
        }
    }
    public class FeedListCategory : ViewModelBase
    {
        public string Category { get; set; }
        public ICommand GetList { get; set; }
        public FeedListCategory(CategoryList list)
        {
            Category = list.category;
            GetList = new RelayCommand(param => getList(list.xmlUrl));
        }
        private void getList(string xmlUrl)
        {
            SearchRSSViewModel vm = SearchRSSViewModel.getInstance();
            vm.getSearchResult(xmlUrl);
        }
    }
    class SearchRSSViewModel : ViewModelBase
    {
        private int _viewHeight, _viewWidth, _categoryColWidth, _categoryListColWidth;
        

        private ObservableCollection<FeedList> _MyData = new ObservableCollection<FeedList>();
        private List<FeedListCategory> _MyCategory = new List<FeedListCategory> ();
        private List<CategoryList> _Category = new List<CategoryList>();

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
        public int CategoryColWidth
        {
            get
            {
                return _categoryColWidth;
            }
            set
            {
                _categoryColWidth = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CategoryColWidth"));
            }
        }
        public int CategoryListColWidth
        {
            get
            {
                return _categoryListColWidth;
            }
            set
            {
                _categoryListColWidth = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CategoryListColWidth"));
            }
        }

        public ObservableCollection<FeedList> MyData
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
        public List<FeedListCategory> MyCategory
        {
            get
            {
                return _MyCategory;
            }
            set
            {
                _MyCategory = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MyCategory"));
            }
        }
        public static SearchRSSViewModel _instance = null;
        Logger LOG = Logger.getInstance();

        public static SearchRSSViewModel getInstance()
        {
            if (_instance == null)
                _instance = new SearchRSSViewModel();
           
                return _instance;
        }

        public SearchRSSViewModel()
        {
            resize();
            initDb();
            initCategory();
        }
        private void initDb()
        {
            CategoryList categoryList = new CategoryList();
            categoryList.category = "News";
            categoryList.xmlUrl = "http://hostmyrss.com/feed/mynewsfeed";
            _Category.Add(categoryList);
            categoryList.category = "Technology";
            categoryList.xmlUrl = "http://hostmyrss.com/feed/mytechnology";
            _Category.Add(categoryList);
            categoryList.category = "Sport";
            categoryList.xmlUrl = "http://hostmyrss.com/feed/mysport";
            _Category.Add(categoryList);
            categoryList.category = "Business";
            categoryList.xmlUrl = "http://hostmyrss.com/feed/mybusiness";
            _Category.Add(categoryList);
            categoryList.category = "Health";
            categoryList.xmlUrl = "http://hostmyrss.com/feed/myfitness";
            _Category.Add(categoryList);
            categoryList.category = "Lifestyle";
            categoryList.xmlUrl = "http://hostmyrss.com/feed/myculture";
            _Category.Add(categoryList);
        }
        public void resize()
        {
            ViewHeight=MainViewModelLocator._height;
            ViewWidth = MainViewModelLocator._width;
            CategoryColWidth = (int)(ViewWidth * 0.3);
            CategoryListColWidth = ViewWidth - CategoryColWidth;


        }
        private void initCategory()
        {
            for (int list = 0; list < _Category.Count; list++)
            {
                _MyCategory.Add(new FeedListCategory(_Category[list]));
            }
            MyCategory = _MyCategory;
        }
        public void getSearchResult(string xmlUrl)
        {
            string title, description, url, logo;
            XmlDocument rssXml = new XmlDocument();
            rssXml.Load(xmlUrl);
            XmlNodeList nodes = rssXml.SelectNodes("//item");

            _MyData.Clear();

            foreach (XmlNode node in nodes)
            {
                title = node["title"].InnerText;
                //description = node["description"].InnerText;
                description = "";
                url = node["link"].InnerText;
                logo = node["content:encoded"].InnerText;

                _MyData.Add(new FeedList(title, description, url, logo));
            }

            MyData = _MyData;
        }
    }
}
