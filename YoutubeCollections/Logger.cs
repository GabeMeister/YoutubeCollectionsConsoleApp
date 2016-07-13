using System;
using System.IO;

namespace YoutubeCollections
{
    public class Logger
    {
        private static Logger _logger = null;
        private static readonly object padlock = new object();
        private readonly string _logFile;

        public Logger()
        {
            _logFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\YoutubeCollectionsLog.log";
        }

        public static Logger Instance
        {
            get
            {
                lock (padlock)
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
            using (var writer = File.AppendText(_logFile))
            {
                writer.WriteLine("{0}:    {1}", DateTime.Now, message);
            }
        }
    }
}
