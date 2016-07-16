using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using YoutubeCollections.ObjectHolders;
using Google.Apis.YouTube.v3.Data;
using YoutubeCollections.Api;
using YoutubeCollections.Database;
using YoutubeCollections.LogParsing;
using YoutubeCollections.LogParsing.LogItems;


namespace YoutubeCollections
{
    public class YoutubeTasks
    {
        #region API

        public static ChannelHolder FetchChannelInfoFromApi(string youtubeId)
        {
            ChannelHolder channel = null;
            ChannelListResponse channelResponse = YoutubeApiHandler.FetchUploadsPlaylistByChannel(youtubeId, "snippet,contentDetails,statistics");

            if (channelResponse.Items != null && channelResponse.Items.Count > 0)
            {
                channel = new ChannelHolder(channelResponse.Items[0]);
            }

            return channel;
        }

        public static List<VideoHolder> FetchVideoInfoFromApi(List<string> videoIdsToFetch)
        {
            var videoInfoList = new List<VideoHolder>();

            while (videoIdsToFetch.Any())
            {
                IEnumerable<string> first50VideoIds = videoIdsToFetch.Take(50);
                VideoListResponse videos = YoutubeApiHandler.FetchVideosByIds(string.Join(",", first50VideoIds), "snippet,contentDetails,statistics");

                foreach (var videoResponse in videos.Items)
                {
                    videoInfoList.Add(new VideoHolder(videoResponse));
                }

                // Remove the first 50 videos
                int removeSize = videoIdsToFetch.Count >= 50 ? 50 : videoIdsToFetch.Count;
                videoIdsToFetch.RemoveRange(0, removeSize);
            }
            
            return videoInfoList;
        }


        #endregion

        #region Channels
        public static void InsertChannelIntoDatabase(ChannelHolder channel)
        {
            Console.WriteLine("INSERTING {0} | {1}", channel.Title, channel.YoutubeId);
            DbHandler.InsertChannel(channel);
        }

        public static void ThreadedFetchChannelsToDownloadUploads()
        {
            List<int> allChannelsToDownloadIds = DbHandler.SelectChannelsToDownloadIds();

#if DEBUG
            int maxIndex = 2;
#else
            int maxIndex = allChannelsToDownloadIds.Count - 1;
#endif

            var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };
            Parallel.For(0, maxIndex, options, i =>
            {
                DbHandler.DeleteChannelToDownload(allChannelsToDownloadIds[i]);
                FetchNewUploadsForChannel(allChannelsToDownloadIds[i]);
            });
            
        }

        public static void ThreadedFetchNewUploadsForExistingChannels()
        {
            List<int> allChannels = DbHandler.SelectAllChannelIds();

#if DEBUG
            int maxIndex = 2;
#else
            int maxIndex = allChannels.Count - 1;
#endif

            var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };
            Parallel.For(0, maxIndex, options, i =>
            {
                FetchNewUploadsForChannel(allChannels[i]);
            });
        }

        public static void FetchNewUploadsForChannel(int channelId)
        {
            // The channel id needs to already be in the database for us to fetch it's uploads
            if (!DbHandler.DoesIdExist("Channels", "ChannelID", channelId))
            {
                return;
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            ChannelHolder channel = DbHandler.PopulateChannelHolderFromTable(channelId);

            string nextPageToken = "";

            var allChannelVideos = new HashSet<string>(DbHandler.SelectAllYoutubeVideoIdsForChannel(channel.ChannelHolderId));
            var videoIdsToFetch = new List<string>();

            bool isSuccess = false;

            do
            {
                isSuccess = false;
                int attempts = 0;

                // We try 3 times to get a successful response
                while (attempts < 3 && !isSuccess)
                {
                    try
                    {
                        var searchListResponse = YoutubeApiHandler.FetchVideosByPlaylist(channel.UploadPlaylist, nextPageToken, "snippet");
                        nextPageToken = searchListResponse.NextPageToken;

                        if (searchListResponse.Items != null && searchListResponse.Items.Count > 0)
                        {
                            foreach (var searchResult in searchListResponse.Items)
                            {
                                string youtubeVideoId = searchResult.Snippet.ResourceId.VideoId;
                                if (allChannelVideos.Contains(youtubeVideoId))
                                {
                                    allChannelVideos.Remove(youtubeVideoId);
                                }
                                else
                                {
                                    videoIdsToFetch.Add(youtubeVideoId);
                                }
                            }

                        }
                        else
                        {
                            Util.PrintAndLog(string.Format("Notice: No uploads found for {0} ({1})", channel.Title, channel.YoutubeId),
                                LogFiles.Instance.ChannelFetchesLogFile);
                        }

                        isSuccess = true;
                    }
                    catch (Exception)
                    {
                        attempts++;
                        Util.PrintAndLog(string.Format("Error: Exception on {0} ({1}) with {2} as page token. Attempt #{3}", channel.Title, channel.YoutubeId, nextPageToken, attempts),
                            LogFiles.Instance.ChannelFetchesLogFile);
                    }
                }
            }
            while (nextPageToken != null && isSuccess);

            // At this point, the video ids left in the list of video ids from the database can be removed,
            // as they weren't returned in the api. They must have been taken down.
            foreach (string videoYoutubeId in allChannelVideos)
            {
                DbHandler.DeleteVideoByYoutubeId(videoYoutubeId);
            }

            // Actually insert new uploads
            List<VideoHolder> videoInfoList = FetchVideoInfoFromApi(videoIdsToFetch);
            foreach (var videoInfo in videoInfoList)
            {
                DbHandler.InsertVideo(videoInfo);
            }

            // Update the channel video count
            ChannelHolder updatedChannelInfo = FetchChannelInfoFromApi(channel.YoutubeId);
            if (updatedChannelInfo != null)
            {
                updatedChannelInfo.ChannelHolderId = channel.ChannelHolderId;
                DbHandler.UpdateChannelInfo(updatedChannelInfo);
                stopWatch.Stop();

                Util.PrintAndLog(string.Format("ChannelFetch: Title={0},VidCount={1},TimeToComplete={2}",
                    channel.Title.Replace(",", ""), updatedChannelInfo.VideoCount, stopWatch.Elapsed.ToString(@"hh\:mm\:ss")),
                    LogFiles.Instance.ChannelFetchesLogFile);
            }
            else
            {
                stopWatch.Stop();
            }
            
        }
        
        public static void FetchAllUploadsForAllChannelSubscriptions(string youtubeId)
        {
            int subscriptionCount = 0;
            string nextPageToken = "";
            SubscriptionListResponse subscriptionsList;

            do
            {
                subscriptionsList = YoutubeApiHandler.FetchSubscriptionsByChannel(youtubeId, nextPageToken, "snippet");
                subscriptionCount += subscriptionsList.Items.Count;
                nextPageToken = subscriptionsList.NextPageToken;

                if (subscriptionsList != null)
                {
                    foreach (var searchResult in subscriptionsList.Items)
                    {
                        Console.WriteLine(searchResult.Snippet.Title);
                        //FetchNewUploadsForChannel(searchResult.Snippet.ResourceId.ChannelId);
                    }
                }
            }
            while (nextPageToken != null);

            Console.WriteLine("Total Subscription Count: " + subscriptionCount);
        }
        
        public static void UpdateAllMissingChannelUploads()
        {
            // TODO: refactor functions to get object type from table
            //// Get all channel youtube ids
            //List<ObjectHolder> allYoutubeChannelIds = DbHandler.SelectColumnsFromTable(typeof(ChannelHolder), "YoutubeID,Title", "Channels");

            //int count = 1;
            //// API request 1 video
            //foreach (ObjectHolder objHolder in allYoutubeChannelIds)
            //{
            //    ChannelHolder channel = objHolder as ChannelHolder;

            //    if (!AreUploadsUpToDate(channel.YoutubeId))
            //    {
            //        Console.WriteLine(count++ + ". " + channel.Title + " out of date. Fetching latest uploads...");

            //        using (StreamWriter writer = File.AppendText(YOUTUBE_LOG_FILE))
            //        {
            //            writer.WriteLine("Fetching latest uploads for " + channel.Title);
            //        }

            //        FetchMissingChannelUploads(channel.YoutubeId);
            //    }
            //    else
            //    {
            //        Console.WriteLine(count++ + ". " + channel.Title + " is up to date!");
            //    }
            //}
        }

        public static bool AreUploadsUpToDate(string youtubeId)
        {
            // To check if uploads are up to date, we just have to request 1 video from the channel's uploads. 
            // If we have that video, then uploads are up to date
            bool status = false;

            // Get the uploads playlist id
            string uploadPlaylistId = DbHandler.SelectColumnBySingleCondition("UploadPlaylist", "Channels", "YoutubeID", youtubeId);

            // Fetch 1 video from the uploads playlist id
            PlaylistItemListResponse response = YoutubeApiHandler.FetchVideosByPlaylist(uploadPlaylistId, "", "snippet", 1);

            if (response != null && response.Items.Count > 0)
            {
                string latestVideoId = response.Items[0].Snippet.ResourceId.VideoId;

                // Query database for latest video
                status = DbHandler.DoesIdExist("Videos", "YoutubeID", latestVideoId);
            }

            return status;
        }

        public static void ProcessChannelFetchesLogFile()
        {
            List<LogItem> logItems = LogParser.ParseLogFile(LogFiles.Instance.ChannelFetchesLogFile);

            List<ChannelFetchLogItem> channelFetches = logItems.Where(x => x.ItemType == "ChannelFetch")
                                                                .Select(x => x as ChannelFetchLogItem)
                                                                .ToList();
            // Sort by channel videos descending to see channels with most uploads first
            channelFetches = channelFetches.OrderByDescending(x => x.VideoCount).ToList();

            int totalVideosFetched = channelFetches.Sum(channel => channel.VideoCount);

            TimeSpan totalTimeSpent = new TimeSpan();
            FinishedTaskLogItem finishedTaskItem = logItems.Find(channel => channel.ItemType == "FinishedTask") as FinishedTaskLogItem;
            if (finishedTaskItem != null)
            {
                totalTimeSpent = finishedTaskItem.ElapsedTime;
            }

            using (var writer = new StreamWriter(LogFiles.Instance.ChannelFetchesReportFile))
            {
                writer.WriteLine("Total Videos Fetched: {0}", totalVideosFetched);
                writer.WriteLine("Total Execution Time: {0} days, {1} hours, {2} minutes", totalTimeSpent.Days, totalTimeSpent.Hours, totalTimeSpent.Minutes);
                writer.WriteLine("Videos per Hour: {0}", (int)(totalVideosFetched / totalTimeSpent.TotalHours));
                writer.WriteLine("DateTime Completed: {0}", DateTime.Now);
                writer.WriteLine();

                foreach (var channel in channelFetches)
                {
                    writer.WriteLine(channel);
                }
            }
        }

        #endregion

        #region Videos
        
        public static void ThreadedUpdateAllVideoInfo()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            List<string> allYoutubeVideoIds = DbHandler.SelectAllVideoYoutubeIds();

            int totalVideos = allYoutubeVideoIds.Count;

            // We need to break down the video list into batches of 50 to query the api with
            List<List<string>> allVideoBatches = new List<List<string>>();
            while (allYoutubeVideoIds.Any())
            {
                int amount = allYoutubeVideoIds.Count > 50 ? 50 : allYoutubeVideoIds.Count;
                allVideoBatches.Add(allYoutubeVideoIds.Take(amount).ToList());
                allYoutubeVideoIds.RemoveRange(0, amount);
            }

            Util.PrintAndLog(string.Format("StartTask: *** Fetching video info for {0} Videos ***", totalVideos), LogFiles.Instance.VideoInfoFetchesLogFile);

            var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };

#if DEBUG
            int maxIndex = 2;
#else
            int maxIndex = allVideoBatches.Count - 1;
#endif
            Parallel.For(0, maxIndex, options, i =>
            {
                List<VideoHolder> videoInfo = FetchVideoInfoFromApi(allVideoBatches[i]);
                foreach (VideoHolder video in videoInfo)
                {
                    Util.PrintAndLog(string.Format("Notice: Updating {0}", video.Title), LogFiles.Instance.VideoInfoFetchesLogFile);
                    DbHandler.UpdateVideoInfo(video);
                }
            });

            stopWatch.Stop();
            Util.PrintAndLog(string.Format("FinishedTask: Message=Finished updating video info for {0} videos,ElapsedTime={1}", 
                totalVideos, stopWatch.Elapsed.ToString(@"hh\:mm\:ss")), LogFiles.Instance.VideoInfoFetchesLogFile);

        }


        #endregion

        
    }
}
