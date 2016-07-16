using System;
using System.IO;

namespace YoutubeCollections.LogParsing
{
    public sealed class LogFiles
    {
        private static LogFiles _logFiles = null;
        public string LogFilesFolder;
        public string DefaultLogFile;
        public string ChannelFetchesLogFile;
        public string ChannelFetchesReportFile;
        public string VideoInfoFetchesLogFile;

        private static readonly object _padLock = new object();

        private LogFiles()
        {
            LogFilesFolder = Environment.CurrentDirectory + @"\Logs";
            if (Directory.Exists(LogFilesFolder))
            {
                Directory.Delete(LogFilesFolder, true);
            }
            Directory.CreateDirectory(LogFilesFolder);

            DefaultLogFile = LogFilesFolder + @"\YoutubeCollectionsLog.log";
            File.WriteAllText(DefaultLogFile, "");

            ChannelFetchesLogFile = LogFilesFolder + @"\ChannelFetches.log";
            File.WriteAllText(ChannelFetchesLogFile, "");

            ChannelFetchesReportFile = LogFilesFolder + @"\ChannelFetchReport.txt";
            File.WriteAllText(ChannelFetchesReportFile, "");

            VideoInfoFetchesLogFile = LogFilesFolder + @"\VideoInfoFetches.log";
            File.WriteAllText(VideoInfoFetchesLogFile, "");

        }

        public static LogFiles Instance
        {
            get
            {
                lock (_padLock)
                {
                    if (_logFiles == null)
                    {
                        _logFiles = new LogFiles();
                    }
                }

                return _logFiles;
            }
        }
    }
}
