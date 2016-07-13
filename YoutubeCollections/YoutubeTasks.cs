using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using YoutubeCollections.ObjectHolders;
using Google.Apis.YouTube.v3.Data;
using System.Threading;


namespace YoutubeCollections
{
    public class YoutubeTasks
    {
        private static int taskCount = 0;
        private const string YOUTUBE_LOG_FILE = @"C:\Users\Gabe\Desktop\YTCollections Dump.log";

        public static void ThreadedFetchExistingChannelUploads()
        {
            List<string> allChannels = DBHandler.FetchChannelsSortedByVideos();

            Parallel.For(0, allChannels.Count, i =>
            {
                FetchChannelUploads(allChannels[i]);
            });

        }

        public static void FetchMissingChannelUploads(string youtubeChannelId)
        {
            // Check if completely new channel
            if (!DBHandler.DoesIdExist("Channels", "YoutubeID", youtubeChannelId))
            {
                // Channel doesn't exist. Throw an exception because this function will only
                // query for videos 5 at a time
                throw new Exception("FetchMissingChannelUploads(): Unrecognized youtube channel id: " + youtubeChannelId);
            }

            // Fetch actual channel id from youtube channel id
            int channelId = DBHandler.RetrieveIdFromYoutubeId("ChannelID", "Channels", youtubeChannelId);

            // Check if completely new channel with no videos written to database
            if (!DBHandler.DoesIdExist("Videos", "ChannelID", channelId))
            {
                // For channels with no videos previously written to database, we just fetch
                // all the channel uploads
                FetchChannelUploads(youtubeChannelId);
            }
            else
            {
                // For channels that have most of their videos inserted but the channel has a few more videos, 
                // we execute this code

                bool upToDate = false;
                string pageToken = string.Empty;
                int newVideoCount = 0;

                // Get the uploads playlist
                string uploadPlaylistId = DBHandler.RetrieveColumnBySingleCondition("UploadPlaylist", "Channels", "YoutubeID", youtubeChannelId);

                do
                {
                    // Fetch 5 videos at a time
                    PlaylistItemListResponse response = YoutubeApiHandler.FetchVideosByPlaylist(uploadPlaylistId, pageToken, "snippet", 50);
                    pageToken = response.NextPageToken;

                    foreach (PlaylistItem item in response.Items)
                    {
                        string youtubeVideoId = item.Snippet.ResourceId.VideoId;

                        // Check if video is in database
                        if (!DBHandler.DoesIdExist("Videos", "YoutubeID", youtubeVideoId))
                        {
                            // Perform api call and write to database
                            FetchVideoInfo(youtubeVideoId);
                            newVideoCount++;
                        }
                        else
                        {
                            upToDate = true;
                            break;
                        }
                    }

                }
                while (!upToDate && pageToken != null);

                Console.WriteLine("Found " + newVideoCount + " new video(s)");
            }


        }

        public static void FetchAllUploadsForAllChannelSubscriptions(string youtubeId)
        {
            int subscriptionCount = 0;
            string nextPageToken = string.Empty;
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
                        //FetchChannelUploads(searchResult.Snippet.ResourceId.ChannelId);
                    }
                }
            }
            while (nextPageToken != null);

            Console.WriteLine("Total Subscription Count: " + subscriptionCount);
        }

        public static void FetchChannelUploads(string youtubeId)
        {
            // TODO: split up the fetch/insert channel code from the update existing uploads code
            int vidCount = 0;
            ChannelHolder channel = InsertChannelIntoDatabaseFromApiResponse(youtubeId);
            if (channel == null)
            {
                // The channel holder will be null if the channel doesn't exist anymore. The channel could
                // have been terminated because of copyright infringement, etc.
                return;
            }

            Console.WriteLine("INSERTING {0} | {1}", channel.Title, channel.YoutubeId);

            string nextPageToken = string.Empty;
            string uploadsPlaylistId = channel.UploadPlaylist;

            do
            {
                try
                {
                    var searchListResponse = YoutubeApiHandler.FetchVideosByPlaylist(uploadsPlaylistId, nextPageToken, "snippet");
                    vidCount += searchListResponse.Items.Count;
                    nextPageToken = searchListResponse.NextPageToken;

                    var videoIds = "";

                    if (searchListResponse.Items != null && searchListResponse.Items.Count > 0)
                    {
                        foreach (var searchResult in searchListResponse.Items)
                        {
                            videoIds += searchResult.Snippet.ResourceId.VideoId + ",";
                        }

                        // Remove last comma
                        videoIds = videoIds.Substring(0, videoIds.Length - 1);

                        FetchVideoInfo(videoIds);
                    }
                }
                catch (Exception e)
                {
                    // Log the error and attempt the api query again
                    using (StreamWriter writer = File.AppendText(YOUTUBE_LOG_FILE))
                    {
                        writer.WriteLine("Error on " + channel.Title + " with " + nextPageToken + " as page token");
                    }
                }
                
            }
            while (nextPageToken != null);

            Console.WriteLine("Total Video Count: " + vidCount);

        }

        public static void MultiThreadedFetchAllChannelUploads()
        {
            // TODO
            //Thread t1 = new Thread(() => YoutubeTasks.FetchChannelUploads(AssociatedPress));
            //Thread t2 = new Thread(() => YoutubeTasks.FetchChannelUploads(WildFilmsIndia));
            //Thread t3 = new Thread(() => YoutubeTasks.FetchChannelUploads(TheYoungTurks));
            //Thread t4 = new Thread(() => YoutubeTasks.FetchChannelUploads(TVCultura));
            //Thread t5 = new Thread(() => YoutubeTasks.FetchChannelUploads(TheTelegraph));
            //Thread t6 = new Thread(() => YoutubeTasks.FetchChannelUploads(TomoNewsUS));

            //t1.Start();
            //t2.Start();
            //t3.Start();
            //t4.Start();
            //t5.Start();
            //t6.Start();

            //t1.Join();
            //t2.Join();
            //t3.Join();
            //t4.Join();
            //t5.Join();
            //t6.Join();
        }

        public static ChannelHolder InsertChannelIntoDatabaseFromApiResponse(string youtubeId)
        {
            ChannelHolder channel = null;
            ChannelListResponse channelResponse = YoutubeApiHandler.FetchUploadsPlaylistByChannel(youtubeId, "snippet,contentDetails,statistics");

            if (channelResponse.Items != null && channelResponse.Items.Count > 0)
            {
                channel = new ChannelHolder(channelResponse.Items[0]);
                DBHandler.InsertChannel(channel);
            }
            
            return channel;
        }

        public static void FetchVideoInfo(string videoIds)
        {
            VideoListResponse videos = YoutubeApiHandler.FetchVideoById(videoIds, "snippet,contentDetails,statistics");

            foreach(var videoResponse in videos.Items)
            {
                VideoHolder video = new VideoHolder(videoResponse);
                DBHandler.InsertVideo(video);

                Console.WriteLine("{0}: {1}", video.YoutubeChannelId, video.Title);
            }

            //Console.Write("█");
            
        }

        public static void RecordChannelSubscriptions(string subscriberYoutubeId)
        {
            int subscriptionCount = 0;
            string nextPageToken = string.Empty;
            SubscriptionListResponse subscriptionsList;

            // Get actual channel id for the subscriber youtube channel
            int subscriberChannelId = DBHandler.RetrieveIdFromYoutubeId("ChannelID", "Channels", subscriberYoutubeId);
            if (subscriberChannelId == -1)
            {
                // Channel hasn't been inserted into database yet.
                InsertChannelIntoDatabaseFromApiResponse(subscriberYoutubeId);
                subscriberChannelId = DBHandler.RetrieveIdFromYoutubeId("ChannelID", "Channels", subscriberYoutubeId);
            }


            do
            {
                subscriptionsList = YoutubeApiHandler.FetchSubscriptionsByChannel(subscriberYoutubeId, nextPageToken, "snippet");
                subscriptionCount += subscriptionsList.Items.Count;
                nextPageToken = subscriptionsList.NextPageToken;

                if (subscriptionsList != null)
                {
                    foreach (var searchResult in subscriptionsList.Items)
                    {
                        string title = searchResult.Snippet.Title;
                        string beingSubscribedToYoutubeId = searchResult.Snippet.ResourceId.ChannelId;

                        

                        // Get actual channel id for the subscriber youtube channel
                        int beingSubscribedToChannelId = DBHandler.RetrieveIdFromYoutubeId("ChannelID", "Channels", beingSubscribedToYoutubeId);
                        if (beingSubscribedToChannelId == -1)
                        {
                            // Channel hasn't been inserted into database yet.
                            InsertChannelIntoDatabaseFromApiResponse(beingSubscribedToYoutubeId);
                            beingSubscribedToChannelId = DBHandler.RetrieveIdFromYoutubeId("ChannelID", "Channels", beingSubscribedToYoutubeId);
                        }

                        Console.WriteLine("Storing subscription to " + title + "...");
                        DBHandler.InsertSubscription(subscriberChannelId, beingSubscribedToChannelId);

                        
                    }
                }
            }
            while (nextPageToken != null);

            Console.WriteLine("Total Subscription Count: " + subscriptionCount);


        }

        public static void DetectChannelSubscriptions()
        {
            List<ObjectHolder> allYoutubeChannelIds = DBHandler.RetrieveColumnsFromTable(typeof(ChannelHolder), "YoutubeID,Title", "Channels");

            foreach (ObjectHolder apiHolder in allYoutubeChannelIds)
            {
                ChannelHolder channel = apiHolder as ChannelHolder;
                bool status = YoutubeApiHandler.DoesChannelHavePublicSubscriptions(channel.YoutubeId);

                if (status)
                {
                    //Console.WriteLine(channel.Title.PadRight(40) + channel.YoutubeId.PadRight(20) + " PUBLIC!");
                }
                else
                {
                    Console.WriteLine(channel.Title.PadRight(40) + channel.YoutubeId.PadRight(20) + " private");
                }
            }
        }

        public static void UpdateAllMissingChannelUploads()
        {
            // Get all channel youtube ids
            List<ObjectHolder> allYoutubeChannelIds = DBHandler.RetrieveColumnsFromTable(typeof(ChannelHolder), "YoutubeID,Title", "Channels");

            int count = 1;
            // API request 1 video
            foreach(ObjectHolder objHolder in allYoutubeChannelIds)
            {
                ChannelHolder channel = objHolder as ChannelHolder;

                if (!AreUploadsUpToDate(channel.YoutubeId))
                {
                    Console.WriteLine(count++ + ". " + channel.Title + " out of date. Fetching latest uploads...");

                    using (StreamWriter writer = File.AppendText(YOUTUBE_LOG_FILE))
                    {
                        writer.WriteLine("Fetching latest uploads for " + channel.Title);
                    }

                    FetchMissingChannelUploads(channel.YoutubeId);
                }
                else
                {
                    Console.WriteLine(count++ + ". " + channel.Title + " is up to date!");
                }
            }
        }

        public static async void UpdateChannelUploadsThreaded(string youtubeChannelId)
        {
            ChannelHolder channel = DBHandler.RetrieveColumnsFromTableById(typeof(ChannelHolder), "YoutubeID,Title", "Channels", youtubeChannelId) as ChannelHolder;

            if (!AreUploadsUpToDate(channel.YoutubeId))
            {
                Console.WriteLine(Interlocked.Increment(ref taskCount) + ". " + channel.Title + " out of date. Fetching latest uploads...");

                using (StreamWriter writer = File.AppendText(YOUTUBE_LOG_FILE))
                {
                    writer.WriteLine("Fetching latest uploads for " + channel.Title);
                }

                await Task.Run(() => FetchMissingChannelUploads(channel.YoutubeId));
            }
            else
            {
                Console.WriteLine(Interlocked.Increment(ref taskCount) + ". " + channel.Title + " is up to date!");
            }
        }
        
        public static bool AreUploadsUpToDate(string youtubeChannelId)
        {
            // To check if uploads are up to date, we just have to request 1 video from the channel's uploads. 
            // If we have that video, then uploads are up to date
            bool status = false;

            // Get the uploads playlist id
            string uploadPlaylistId = DBHandler.RetrieveColumnBySingleCondition("UploadPlaylist", "Channels", "YoutubeID", youtubeChannelId);

            // Fetch 1 video from the uploads playlist id
            PlaylistItemListResponse response = YoutubeApiHandler.FetchVideosByPlaylist(uploadPlaylistId, "", "snippet", 1);

            if (response != null && response.Items.Count > 0)
            {
                string latestVideoId = response.Items[0].Snippet.ResourceId.VideoId;

                // Query database for latest video
                status = DBHandler.DoesIdExist("Videos", "YoutubeID", latestVideoId);
            }

            return status;
        }

        




        

        public static void UpdateAllVideoInfo()
        {
            // TODO: finish
            Queue<string> allVideosQueue = new Queue<string>(DBHandler.RetrieveColumnFromTable("YoutubeID", "Videos"));

            while(allVideosQueue.Count > 0)
            {
                // We do fetches in async threads to speed up the time needed to 
                
                
            }

        }

        public void MarkVideosDownloadedOrNot()
        {
            List<int> allChannelIds = DBHandler.RetrieveColumnFromTable("ChannelID", "Channels").Select(x => int.Parse(x)).ToList();

            foreach (int channelId in allChannelIds)
            {
                bool areVideosPresent = DBHandler.DoesIdExist("Videos", "ChannelID", channelId);
                string channelName = DBHandler.RetrieveColumnBySingleCondition("Title", "Channels", "ChannelID", channelId.ToString());
                DBHandler.SetAreVideosLoadedForChannel(channelId, areVideosPresent);
                Logger.Instance.Log(string.Format("{0}: {1}", channelName, areVideosPresent ? "yes" : "no"));
            }
        }

    }
}
