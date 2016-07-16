using System.Diagnostics;

namespace YoutubeCollections.LogParsing.LogItems
{
    public class NoticeLogItem : LogItem
    {
        public string Message { get; set; }

        public NoticeLogItem(string logStr) : base(logStr)
        {
            Debug.Assert(LogString.Contains("Notice: "), "Error: incompatible log string for Notice log item");

            ItemType = "Notice";
            LogString = LogString.Replace("Notice: ", "");

            Message = LogString;
        }
    }
}
