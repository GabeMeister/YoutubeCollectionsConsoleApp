using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCollections.LogParsing.LogItems
{
    public class FinishedTaskLogItem : LogItem
    {
        public string Message { get; set; }
        public TimeSpan ElapsedTime { get; set; }

        public FinishedTaskLogItem(string logStr): base(logStr)
        {
            Debug.Assert(LogString.Contains("FinishedTask: "), "Error: incompatible log string for FinishedTask log item");

            ItemType = "FinishedTask";
            LogString = LogString.Replace("FinishedTask: ", "");

            string[] tokens = LogString.Split(',');

            foreach (string token in tokens)
            {
                string[] values = token.Split('=');
                string key = values[0];
                string value = values[1];

                switch (key)
                {
                    case "Message":
                        Message = value;
                        break;
                    case "ElapsedTime":
                        ElapsedTime = TimeSpan.Parse(value);
                        break;
                }
            }
        }
    }
}
