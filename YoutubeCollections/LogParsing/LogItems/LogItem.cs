using System;

namespace YoutubeCollections.LogParsing.LogItems
{
    public abstract class LogItem
    {
        public string DateTimeStamp { get; set; }
        public string ItemType { get; set; }
        public string LogString { get; set; }

        protected LogItem(string logStr)
        {
            // Split date time stamp from log string itself
            string[] tokens = logStr.Split(new[] { "    " }, StringSplitOptions.None);
            DateTimeStamp = tokens[0].Replace(":", "");
            LogString = tokens[1];
        }
    }
}
