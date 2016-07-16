using System;
using System.Diagnostics;

namespace YoutubeCollections.LogParsing.LogItems
{
    public class ChannelFetchLogItem : LogItem
    {
        public string Title { get; set; }
        public int VideoCount { get; set; }
        public TimeSpan TimeToComplete { get; set; }
        
        public ChannelFetchLogItem(string logStr) : base(logStr)
        {
            // Example log string: 7/13/2016 9:39:16 PM:    ChannelFetch: Title=SleeplessRecord FR,VidCount=45,TimeToComplete=00:00:28
            Debug.Assert(LogString.Contains("ChannelFetch: "), "Error: incompatible log string for ChannelFetch log item");
            
            ItemType = "ChannelFetch";
            LogString = LogString.Replace("ChannelFetch: ", "");

            string[] logItems = LogString.Split(',');

            foreach (string logItem in logItems)
            {
                string[] dataTokens = logItem.Split('=');
                string key = dataTokens[0];
                string value = dataTokens[1];

                switch (key)
                {
                    case "Title":
                        Title = value;
                        break;
                    case "VidCount":
                        VideoCount = Convert.ToInt32(value);
                        break;
                    case "TimeToComplete":
                        TimeToComplete = TimeSpan.Parse(value);
                        break;
                    default:
                        throw new Exception("Unrecognized channel fetch log item");
                }

            }

        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", Title, VideoCount);
        }
    }
}
