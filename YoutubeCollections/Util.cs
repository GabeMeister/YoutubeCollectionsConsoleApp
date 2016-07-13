using System;

namespace YoutubeCollections
{
    public class Util
    {
        public static void PrintAndLog(string message)
        {
            Console.WriteLine(message);
            Logger.Instance.Log(message);
        }
    }
}
