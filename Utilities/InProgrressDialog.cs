using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FeedNews.Utilities
{
    public static class InProgrressDialog
    {
        private static Window mInProgressDialog;
        public static Window InProgress
        {
            get
            {
                return mInProgressDialog;
            }
            set
            {
                mInProgressDialog = value;
            }
        }
        public static void awakeProcess()
        {
            if (mInProgressDialog != null)
            {
                mInProgressDialog.Show();
            }
        }
        public static void abortProcess()
        {
            if (mInProgressDialog != null)
            {
                mInProgressDialog.Close();

                if (mInProgressDialog is IDisposable)
                    (mInProgressDialog as IDisposable).Dispose();
            }
        }
    }
}
