using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace FeedNews.Models
{   
    public struct xmlData
    {
         public string title;
         public string url;
         public string description;
         public string imageUri;
         public string pubDate;
         public string GUID;
         public bool readFlag;
    }

    public class RSSFeedList
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public string Logo { get; set; }
        public RSSFeedList(string index, string name, string uri, string logo)
        {
            Index = index;
            Name = name;
            Uri = uri;
            Logo = logo;
        }
    }
    public class RSSModel
    {
        private const string RSS_FEED_LIST = "rssFeed.config";
        private const string GUID_LIST = "guidId.config";

        public static int gNum = 0;
        public static int gTotalItems = 0;
        public static int gFeederId = 0;
        public static int gMaxFeeder = 0;
        public static RSSModel instance = null;

        public static List<xmlData> listXmlContent;
        public ObservableCollection<RSSFeedList> rssFeedList = new ObservableCollection<RSSFeedList>();
        public static WebClient client = new WebClient();
        public static System.Windows.Forms.WebBrowser webControl = new System.Windows.Forms.WebBrowser();
        XmlDocument news = new XmlDocument();
        Logger log = Logger.getInstance();

        private Object thisLock = new Object();
        private Object fileLock = new Object();

        public RSSModel()
        {
            log.LoggerPrint("[RSSModel::RSSModel] Start");
            getRssList();
            getXmlData();
            getXmlNodeCnt();
        }
        public static RSSModel getInstance()
        {
            if (instance == null)
            {
                instance = new RSSModel();
            }

            return instance;
        }
        public void getXmlData()
        {
            if (!File.Exists(getFileName()))
            {
                log.LoggerPrint("[RSSModel::getXmlData] " + RSS_FEED_LIST + " is empty");
                return;
            }

            if (rssFeedList[gFeederId].Uri == null)
            {
                log.LoggerPrint("[RSSModel::getXmlData] rssFeedList[" + gFeederId + "].Uri is null");
                return;
            }

            lock (thisLock)
            {
                news = new XmlDocument();
                news.Load(rssFeedList[gFeederId].Uri);
                log.LoggerPrint("[RSSModel::getXmlData] load rssFeedList::idx=" + rssFeedList[gFeederId].Index
                                    + " name=" + rssFeedList[gFeederId].Name);

                if (listXmlContent == null)
                    listXmlContent = new List<xmlData>();
                else
                    listXmlContent.Clear();


                XmlNodeList nodes = news.SelectNodes("//item");
                gTotalItems = nodes.Count;
                foreach (XmlNode node in nodes)
                {
                    xmlData item = new xmlData();
                    item.title = parseParghTag(node["title"].InnerText);
                    item.url = node["link"].InnerText;
                    item.description = parseParghTag(node["description"].InnerText);

                    if (node["pubDate"] != null)
                        item.pubDate = node["pubDate"].InnerText;
                    else
                        item.pubDate = node["dc:date"].InnerText;
                    try
                    {
                        if (node["enclosure"] != null)
                        {
                            item.imageUri = node["enclosure"].Attributes["url"].Value;
                        }
                        else if (node["media:content"] != null)
                        {
                            item.imageUri = node["media:content"].Attributes["url"].Value;
                        }
                        else if (node["content:encoded"] != null)
                        {
                            item.imageUri = parseForImageUri(node["content:encoded"].InnerText);
                        }
                        else if (node["description"] != null)
                        {
                            item.imageUri = parseForImageUri(node["description"].InnerText);
                        }
                        else
                        {
                            log.LoggerPrint("[RSSModel::getXmlData] image node not found");
                        }
                    }
                    catch
                    {
                        item.imageUri = "";
                    }

                    try
                    {
                        item.GUID = node["guid"].InnerText;
                    }
                    catch
                    {
                        item.GUID = "";
                    }
                    /// check if guid is new
                    /// 
                    item.readFlag = checkGuidExist(item.GUID);

                    listXmlContent.Add(item);
                }
                ///clear memory for xmlDocument load
              //  nodes = null;
            }
        }
        public bool getXmlDataAudit(int feederIdx)
        {
            List<xmlData> auditList = new List<xmlData>();
            XmlDocument newsAudit = new XmlDocument();
            XmlNodeList nodesAudit;

            string guid;
            bool updateflag = false;

            if (!File.Exists(getFileName()))
            {
                log.LoggerPrint("[RSSModel::getXmlDataAudit] " + RSS_FEED_LIST + " is empty");
                return updateflag;
            }

            if (rssFeedList[feederIdx].Uri == null)
            {
                log.LoggerPrint("[RSSModel::getXmlDataAudit] rssFeedList[" + gFeederId + "].Uri is null");
                return updateflag ;
            }
            lock (thisLock)
            {
                newsAudit.Load(rssFeedList[feederIdx].Uri);

                nodesAudit = newsAudit.SelectNodes("//item");
                
                foreach (XmlNode nodeAudit in nodesAudit)
                {
                    try
                    {
                        guid = nodeAudit["guid"].InnerText;
                    }
                    catch
                    {
                        log.LoggerPrint("[RSSModel::getXmlDataAudit] load rssFeedList::idx=" + rssFeedList[feederIdx].Index
                   + " name=" + rssFeedList[feederIdx].Name);
                        log.LoggerPrint("[RSSModel::getXmlDataAudit]audit no guid");
                        continue;
                    }
                    /// check if guid is new
                    /// 
                    if (checkGuidExist(guid))
                    {
                        updateflag = false;
                        auditList.Clear();
                        newsAudit = null;
                        nodesAudit = null; 
                        return updateflag;
                    }
                }
            }

            updateflag = true;
            auditList.Clear();
            newsAudit = null;
            nodesAudit = null; 
            return updateflag;
        }
        public void getXmlNodeCnt()
        {
            if (!File.Exists(getFileName()))
            {
                log.LoggerPrint("[RSSModel::getXmlNodeCnt] " + RSS_FEED_LIST + " is empty");
                return;
            }

            if (rssFeedList[gFeederId].Uri == null)
                return;

            news.Load(rssFeedList[gFeederId].Uri);
            XmlNodeList nodes = news.SelectNodes("//item");
            gTotalItems = nodes.Count;
            log.LoggerPrint("[RSSModel::getXmlNodeCnt] gTotalItems: " + gTotalItems);
        }
        public void getRssList()
        {
            string file = getFileName();
            string[] lines = null;
            gMaxFeeder = 0;
            log.LoggerPrint("[RSSModel::getRssList] start");
            if (rssFeedList == null)
                rssFeedList = new ObservableCollection<RSSFeedList>();
            else
                rssFeedList.Clear();

            if (file == null)
            {
                log.LoggerPrint("rssFeed.config are null");
                Application.Exit();
            }
            
            try
            {
                lines = System.IO.File.ReadAllLines(file);
                foreach (string line in lines)
                {
                    /// skip empty line
                    if (line == null)
                        continue;

                    string[] fields = line.Split(',');
                    
                    /// exception: field[3] might be null
                    try
                    {
                        rssFeedList.Add(new RSSFeedList(fields[0], fields[1], fields[2], fields[3]));
                    }
                    catch
                    {
                        rssFeedList.Add(new RSSFeedList(fields[0], fields[1], fields[2], "http://i41.tinypic.com/sgr220.png"));
                        log.LoggerPrint("[RSSModel::getRssList] Feeder index: " + fields[0] + "has no logo");
                    }
                    gMaxFeeder++;
                }
            }
            catch
            {
                log.LoggerPrint("Fail to read " + file);
                return;
            }
            log.LoggerPrint("[RSSModel::getRssList] End. gMaxFeeder: " + gMaxFeeder);
        }
        public static string getFileName()
        {
            string path = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            return path + @"\" + RSS_FEED_LIST;
        }
        public static string getFileName(string fileName)
        {
            string path = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            return path + @"\" + fileName;
        }
        public string parseForImageUri(string text)
        {
            text = removeChar(text);
            /// check validity
            Match match = Regex.Match(text, @"(http://.*.jpg)");
            if (!match.Success)
            {
                log.LoggerPrint("[RSSModel::parseForImageUri] Invalid imageUrl:");
                return "";
            }
            else
            {
                return match.Groups[1].Value;
            }         
        }
        public string parseParghTag(string text)
        {
            string body = "";
            string tmp = "";
            body = removeChar(text);
            string[] words = body.Split('<');
            foreach (string word in words)
            {
                tmp += Regex.Replace(word, ".*>", "");
            }
            body = tmp;
            return body;
        }
        private string removeChar(string text)
        {
            string result = "";
            result = Regex.Replace(text, "&lt;", "");
            result = Regex.Replace(text, "p&gt;", "");
            result = Regex.Replace(text, "&quot;", "");
            result = Regex.Replace(text, "(&.*;)", "");
            return result;
        }
        private bool checkGuidExist(string guid)
        {
            bool flag = false;
            string [] lines;
            string guidFile = getFileName(GUID_LIST);

            lock (fileLock)
            {
                try
                {
                    lines = System.IO.File.ReadAllLines(getFileName(GUID_LIST));
                    foreach (string line in lines)
                    {
                        if (line == guid)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                catch
                {
                    return flag;
                }
            }
            return flag;
        }

    }
}
