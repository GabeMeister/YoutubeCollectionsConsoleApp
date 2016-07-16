using System.Collections.Generic;
using System.IO;
using YoutubeCollections.LogParsing.LogItems;

namespace YoutubeCollections.LogParsing
{
    public class LogParser
    {
        public static List<LogItem> ParseLogFile(string logFilePath)
        {
            var logItems = new List<LogItem>();

            using (var reader = new StreamReader(logFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("Notice: "))
                    {
                        logItems.Add(new NoticeLogItem(line));
                    }
                    else if (line.Contains("Error: "))
                    {
                        logItems.Add(new ErrorLogItem(line));
                    }
                    else if (line.Contains("ChannelFetch: "))
                    {
                        logItems.Add(new ChannelFetchLogItem(line));
                    }
                    else if (line.Contains("StartTask: "))
                    {
                        logItems.Add(new StartTaskLogItem(line));
                    }
                    else if (line.Contains("FinishedTask: "))
                    {
                        logItems.Add(new FinishedTaskLogItem(line));
                    }
                }
            }

            return logItems;
        }
    }
}
