using System;
using System.Diagnostics;

namespace YoutubeCollections.LogParsing.LogItems
{
    public class StartTaskLogItem : LogItem
    {
        public string Message { get; set; }

        public StartTaskLogItem(string logStr) : base(logStr)
        {
            Debug.Assert(LogString.Contains("StartTask: "), "Error: incompatible log string for StartTask log item");

            ItemType = "StartTask";
            LogString = LogString.Replace("StartTask: ", "");

            Message = LogString;
        }
    }
}
