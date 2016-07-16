using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCollections.LogParsing.LogItems
{
    public class ErrorLogItem : LogItem
    {
        public string Error { get; set; }

        public ErrorLogItem(string logStr) : base(logStr)
        {
            Debug.Assert(LogString.Contains("Error: "), "Error: incompatible log string for Error log item");

            ItemType = "Error";
            LogString = LogString.Replace("Error: ", "");

            Error = LogString;
        }
    }
}
