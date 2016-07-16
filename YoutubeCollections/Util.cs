using System;
using System.Diagnostics;
using YoutubeCollections.LogParsing;

namespace YoutubeCollections
{
    public class Util
    {
        public static void PrintAndLog(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
            Logger.Instance.Log(message);
        }

        public static void PrintAndLog(string message, string logFile)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
            Logger.Instance.Log(message, logFile);
        }

        public static void Print(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

    }
}
