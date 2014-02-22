using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

using Model = FeedNews.Models.RSSModel;

namespace FeedNews.ViewModels
{
    class FeedNewsHtmlViewModel : ViewModelBase
    {
        private string htmlText;
        Logger LOG = Logger.getInstance();
        public string htmlContent
        {
            get
            {
                return htmlText;
            }
            set
            {
                htmlText = value;
                OnPropertyChanged(new PropertyChangedEventArgs("htmlContent"));
            }
        }



        public FeedNewsHtmlViewModel()
        {
        }

        public void getHtmlContent()
        {
            if (Model.listXmlContent == null)
            {
                LOG.LoggerPrint("[RSSFeedHtmlViewModel::getHtmlContent] listXmlContent is empty.");
                return;
            }
            HtmlParser obj = new HtmlParser();
            obj.getHtmlText(Model.listXmlContent[Model.gNum].url);
            htmlText = obj.htmlText;
        }
       
    }
}
