using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Model = FeedNews.Models.RSSModel;
using BoardViewModel = FeedNews.ViewModels.FeedNewsBoardViewModel;

namespace FeedNews.Utilities
{
    class UpdateChecker
    {
        public const string PUSH_NOTI_ICON = "http://oi59.tinypic.com/2prtq8w.jpg";
        private const int SLEEP = 6000;
        Model obj = Model.getInstance();
        BoardViewModel boardVM = BoardViewModel.getInstance();
        List<FeedNews.ViewModels.Feeder> feederList = new List<FeedNews.ViewModels.Feeder>();
        Logger LOG = Logger.getInstance();

        public UpdateChecker()
        {
            feederList = boardVM.MyData;
            LOG.LoggerPrint("UpdateChecker::UpdateChecker] Update start. threadId=" + Thread.CurrentThread.ManagedThreadId);
        }
        
        public void auditCheckUpdate()
        {
            for (int feedIdx = 0; feedIdx < Model.gMaxFeeder; feedIdx++)
            {
                bool flag = false;
                feederList[feedIdx].UpdateNotiIcon = "";
                flag = obj.getXmlDataAudit(feedIdx);
                
                if (flag)
                {
                    feederList[feedIdx].UpdateNotiIcon = PUSH_NOTI_ICON;
                    //LOG.LoggerPrint("[UpdateChecker::auditCheckUpdate] Feeder:"
                      //              + feederList[feedIdx].Content + " has update");
                }
                Thread.Sleep(1000);
            }
            boardVM.MyData = feederList;
        }
        public void startAuditCheckUpdate()
        {
            while (true)
            {
                auditCheckUpdate();
                Thread.Sleep(60000);
            }
        }
         
    }
}
