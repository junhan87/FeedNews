using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

namespace FeedNews
{
    class Logger
    {
        private const long MAX_LOG_FILE_SIZE = 256000;
        private const string LOG_FILE_NAME = "FeedNews.log";
        private const string LOG_FILE_NAME_BACKUP = "FeedNews.backup.log";
        private static string path = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        string logFile = path + @"\" + LOG_FILE_NAME;
        private Object logFileLock = new Object();

        StreamWriter w;
        public static Logger _instance;
        public static Logger getInstance()
        {
            if (_instance == null)
                _instance = new Logger();

            return _instance;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public Logger()
        {
            /// check if log file exist
            if (!File.Exists(logFile))
            {
                /// create log file
                    LoggerPrint("Firstly Start");
            }
        }
        /// <summary>
        /// print log message
        /// </summary>
        /// <param name="logMessage"></param>
        public void LoggerPrint(string logMessage)
        {
            checkFileSize();

            /// write append to the log file
            lock (logFileLock)
            {
                using (w = File.AppendText(logFile))
                {
                    w.WriteLine("{0} {1} :{2}", DateTime.Now.ToShortDateString(),
                                           DateTime.Now.ToLongTimeString(),
                                           logMessage);
                }
            }
        }

        /// <summary>
        /// check log file size. Backup if exceeds 256Bytes
        /// </summary>
        private void checkFileSize()
        {
            if (!File.Exists(logFile))
            {
                return;
            }

            FileInfo F = new FileInfo(logFile);

            if (F.Length >= MAX_LOG_FILE_SIZE)
            {
                if (File.Exists(LOG_FILE_NAME_BACKUP))
                {
                    System.IO.File.Delete(LOG_FILE_NAME_BACKUP);
                }
                System.IO.File.Move(LOG_FILE_NAME, LOG_FILE_NAME_BACKUP);
                System.IO.File.Delete(LOG_FILE_NAME);
            }
        }
    }
}
