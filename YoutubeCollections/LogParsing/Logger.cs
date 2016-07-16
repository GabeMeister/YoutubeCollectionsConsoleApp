using System;
using System.IO;

namespace YoutubeCollections.LogParsing
{
    public class Logger
    {
        private static Logger _logger = null;
        private static readonly object instancePadLock = new object();
        private static readonly object logFilePadLock = new object();
        private readonly string _logFile;

        public string LogFilePath
        {
            get { return _logFile; }
        }

        public Logger()
        {
            _logFile = LogFiles.Instance.DefaultLogFile;
            File.WriteAllText(_logFile, "");
        }

        public static Logger Instance
        {
            get
            {
                lock (instancePadLock)
                {
                    if (_logger == null)
                    {
                        _logger = new Logger();
                    }
                }
                return _logger;
            }
        }

        public void Log(string message)
        {
            lock (logFilePadLock)
            {
                using (var writer = File.AppendText(_logFile))
                {
                    writer.WriteLine("{0}:    {1}", DateTime.Now, message);
                }
            }
        }

        public void Log(string message, string logFile)
        {
            lock (logFilePadLock)
            {
                using (var writer = File.AppendText(logFile))
                {
                    writer.WriteLine("{0}:    {1}", DateTime.Now, message);
                }
            }
        }

    }
}
