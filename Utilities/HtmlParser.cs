using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Model = FeedNews.Models.RSSModel;

namespace FeedNews
{
    class HtmlParser
    {
        private const int MAX_PARAGH_PATTERN = 4;
        public string htmlText;
        Logger LOG = Logger.getInstance();
        public HtmlParser()
        {
        }
        public void load(string _url)
        {
            LOG.LoggerPrint("[HtmlParser::load] Load " + _url);
            // check if url valid
            try
            {
                getHTMLString(_url);
            }
            catch
            {
                LOG.LoggerPrint("[HtmlParser::load] " + _url + " is invalid");
                htmlText = "";
                return;
            }
            // Create a request for the URL. 		
            WebRequest request = WebRequest.Create(_url);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content. 
            htmlText = reader.ReadToEnd();
            LOG.LoggerPrint("[HtmlParser::load] got stream");                
            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();

        }
        public string getHTMLString(string Url)
        {
            return Model.client.DownloadString(Url);
        }
        public void getFeederIcon(string url)
        {
            string address;
            string[] words = url.Split('/');

            ///http://cnn.com
            try
            {
                getHTMLString(words[2]);
                address = words[2];
            }
            catch /// http://feed.cnn.com
            {
                LOG.LoggerPrint("[HtmlParser::getFeederIcon] Splitted Url=" + words[2] + " is invalid");
                string prefix = words[2].Split('.')[0] + ".";
                address = "http://" + words[2].Substring(prefix.Length, words[2].Length - prefix.Length);    
            }
            LOG.LoggerPrint("[HtmlParser::getFeederIcon] address: " + address);
            load(address);
            parseMetaTag(htmlText);
        }
        public void getHtmlText(string url)
        {
            load(url);
            htmlText = parseParghTag(htmlText);
        }
        private void parseMetaTag(string text)
        {
            string []pattern = new string [2];
            pattern[0] = "<meta" + @".*" + "image" + '"' + " content=" + '"' + "(.*)" + '"' + @".*/>";
            pattern[1] = "<link" + @" .* " + "href=" + '"' + "(.*.ico)" + '"' + @".*/>";

            string[] subPattern = new string[2];
            subPattern[0] = @"(.*)" + '"' + @".*/>";
            subPattern[1] = @"(http.*.ico)" + '"';
            for (int i = 0; i < 2; i++)
            {
                Match match = Regex.Match(text, pattern[i], RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    htmlText = match.Groups[1].Value;
                    match = Regex.Match(match.Groups[1].Value, subPattern[i]);
                    if (match.Success)
                    {
                        htmlText = match.Groups[1].Value;
                        LOG.LoggerPrint("[HtmlParser::parseMetaTag] htmlText=" + htmlText);
                    }
                    break;
                }  
            }

            try
            {
                getHTMLString(htmlText);
            }
            catch
            {
                htmlText = "http://i41.tinypic.com/sgr220.png";
            }
        }
        public string parseParghTag(string text)
        {
            string body = "";
            string[] pattern = new string[2];

            pattern[0] = @"<p>(.*)</p>";
            pattern[1] = @"<p>[\n\r]+(.*)</p>";

            if (text == null)
            {
                body = Model.listXmlContent[Model.gNum].description;
                return body;
            }

            MatchCollection matches;

            for (int i = 0; i < 2; i++)
            {
                if (body.Length > 0)
                    break;

                matches = Regex.Matches(text, pattern[i], RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    string[] words = match.Groups[1].Value.Split('<');
                    foreach (string word in words)
                    {
                        string tmp;
                        tmp = removeChar(word);
                        body += Regex.Replace(tmp, ".*>", "");
                    }

                    body += "\n\n";
                }
            }
        /*    if (body.Length <= 0)
            {
                matches = Regex.Matches(text, @"<p>[\n\r]+(.*)</p>", RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    LOG.LoggerPrint("DEBUG:" + match.Groups[1].Value);
                    string[] words = match.Groups[1].Value.Split('<');
                    foreach (string word in words)
                    {
                        body += Regex.Replace(word, ".*>", "");
                    }
                    body += "\n\n";
                }
            }*/
           // body = removeChar(body);
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
    }
}
