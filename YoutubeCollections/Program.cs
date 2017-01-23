using System;
using System.Diagnostics;
using YoutubeCollections.Api;
using YoutubeCollections.Database;
using YoutubeCollections.LogParsing;

namespace YoutubeCollections
{
    internal class YoutubeCollectionsApp
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"DATABASE CONNECTION STRING: {DbHandler.DatabaseConnStr}");

            try
            {
                FetchAllUploadsForChannelsInCollections();
                FetchAllChannelsToDownloadUploads();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            Console.WriteLine("Done.");
            Console.ReadLine();

            //var s = YoutubeApiHandler.FetchUploadsPlaylistByChannel("UU4LVLoBN0xbOb5xJuA0ia9A4", "snippet");
        }

        static void FetchAllUploadsForChannelsInCollections()
        {
            // We fetch the uploads of channels in the ChannelsToDownload table first,
            // because those channels we know have no videos.
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            Util.PrintAndLog("StartTask: *** Fetching uploads for all channels found in collections***", LogFiles.Instance.ChannelFetchesLogFile);

            YoutubeTasks.ThreadedFetchUploadsForChannelsInCollections(LogFiles.Instance.ChannelFetchesLogFile);

            stopWatch.Stop();
            Util.PrintAndLog(
                $"FinishedTask: Message=Fetched all new uploads for channels found in collections,ElapsedTime={stopWatch.Elapsed.ToString(@"hh\:mm\:ss")}",
                LogFiles.Instance.ChannelFetchesLogFile);

            YoutubeTasks.ProcessChannelFetchesLogFile();
        }

        static void FetchAllChannelUploads()
        {
            // We fetch the uploads of channels in the ChannelsToDownload table first,
            // because those channels we know have no videos.
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            Util.PrintAndLog("StartTask: *** Fetching uploads for all channels ***", LogFiles.Instance.ChannelFetchesLogFile);

            YoutubeTasks.ThreadedFetchChannelsToDownloadUploads(LogFiles.Instance.ChannelFetchesLogFile);
            YoutubeTasks.ThreadedFetchNewUploadsForAllChannels(LogFiles.Instance.ChannelFetchesLogFile);

            stopWatch.Stop();
            Util.PrintAndLog(string.Format("FinishedTask: Message=Fetched all new uploads for all channels,ElapsedTime={0}", stopWatch.Elapsed.ToString(@"hh\:mm\:ss")),
                LogFiles.Instance.ChannelFetchesLogFile);

            YoutubeTasks.ProcessChannelFetchesLogFile();
        }

        static void FetchAllChannelsToDownloadUploads()
        {
            // We fetch the uploads of channels in the ChannelsToDownload table first,
            // because those channels we know have no videos.
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            Util.PrintAndLog("StartTask: *** Fetching uploads for all channels to download ***", LogFiles.Instance.ChannelFetchesLogFile);

            YoutubeTasks.ThreadedFetchChannelsToDownloadUploads(LogFiles.Instance.ChannelFetchesLogFile);

            stopWatch.Stop();
            Util.PrintAndLog(string.Format("FinishedTask: Message=Fetched all new uploads for all channels to download,ElapsedTime={0}", stopWatch.Elapsed.ToString(@"hh\:mm\:ss")),
                LogFiles.Instance.ChannelFetchesLogFile);

            YoutubeTasks.ProcessChannelFetchesLogFile();
        }
        
        static void FetchUpdatedVideoInfo()
        {
            YoutubeTasks.ThreadedUpdateAllVideoInfo();
        }
    }
}