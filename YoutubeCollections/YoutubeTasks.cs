﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using Npgsql;
using YoutubeCollections.ObjectHolders;
using YoutubeCollections.Database;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;




namespace YoutubeCollections
{
    public class YoutubeTasks
    {
        private const int MAX_RESULTS = 50;
        private const string YOUTUBE_LOG_FILE = @"C:\Users\Gabe\Desktop\YTCollections Dump.log";

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
            int vidCount = 0;
            ChannelHolder channel = InsertChannelIntoDatabaseFromApiResponse(youtubeId);
            Console.WriteLine("************* " + channel.Title + " | " + channel.YoutubeId + " *************");

            string nextPageToken = string.Empty;
            string uploadsPlaylistId = channel.UploadPlaylist;
            PlaylistItemListResponse searchListResponse;

            do
            {
                try
                {
                    searchListResponse = YoutubeApiHandler.FetchVideosByPlaylist(uploadsPlaylistId, nextPageToken, "snippet");
                    vidCount += searchListResponse.Items.Count;
                    nextPageToken = searchListResponse.NextPageToken;

                    if (searchListResponse != null)
                    {
                        string videoIds = string.Empty;

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
                //DBHandler.InsertVideo(video);

                Console.WriteLine("{0}: {1}", video.YoutubeChannelId, video.Title);
            }

            //Console.Write("█");
            
        }

        public static void FetchChannelUploadsFromStream(StreamReader reader)
        {
            using (reader)
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    string[] tokens = line.Split('\t');
                    string channelId = tokens[0];
                    string youtubeId = tokens[1];
                    string title = tokens[2];

                    FetchChannelUploads(youtubeId);

                }
            }
        }

        public static void WriteAllChannelIdsToStream(StreamWriter writer)
        {
            using (writer)
            {
                string selectAllChannelsSql = SqlBuilder.SelectAllSql("ChannelID,YoutubeID,Title", "Channels");

                using (NpgsqlConnection conn = new NpgsqlConnection(DBHandler.DatabaseConnStr))
                {
                    conn.Open();
                    
                    NpgsqlCommand command = new NpgsqlCommand(selectAllChannelsSql, conn);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    
                    while(reader.Read())
                    {
                        string channelId = reader["ChannelID"].ToString().Trim();
                        string youtubeId = reader["YoutubeID"].ToString().Trim();
                        string title = reader["Title"].ToString().Trim();
                        writer.WriteLine("{0}\t{1}\t{2}", channelId, youtubeId, title);
                    }

                    conn.Close();
                }

                
            }
        }

        public static void AddPublishedAtTimeStamps()
        {
            try
            {
                using (var conn = new NpgsqlConnection(DBHandler.DatabaseConnStr))
                {
                    conn.Open();

                    // We check if the same youtube channel id has already been inserted
                    string selectSql = "select YoutubeID from videos where publishedAt is NULL;";
                    NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                    NpgsqlDataReader reader = selectCommand.ExecuteReader();

                    List<string> youtubeIds = new List<string>();
                    VideoListResponse videoResponse = null;

                    while(reader.Read())
                    {
                        youtubeIds.Add(reader["YoutubeId"].ToString());
                    }

                    reader.Close();

                    int count = 0;
                    int totalCount = 0;
                    string videoList = string.Empty;
                    // API call with each video id
                    foreach(string youtubeId in youtubeIds)
                    {
                        if (count == 49)
                        {
                            // To make it 50
                            videoList += youtubeId.Trim();
                            count++;

                            videoResponse = YoutubeApiHandler.FetchVideoById(videoList, "snippet");

                            foreach(Video video in videoResponse.Items)
                            {
                                DateTime? publishedAt = video.Snippet.PublishedAt;

                                string udpateSql = string.Format("update videos set PublishedAt='{0}' where YoutubeID='{1}';", publishedAt.Value.ToString("yyyy-MM-dd HH:MM:ss").Trim(), video.Id.Trim());

                                NpgsqlCommand updateCommand = new NpgsqlCommand(udpateSql, conn);
                                int rowsAffected = updateCommand.ExecuteNonQuery();

                                if (rowsAffected < 1)
                                {
                                    throw new Exception("Video update didn't complete correctly.");
                                }
                            }


                            
                            videoList = string.Empty;
                            totalCount += count;
                            Console.WriteLine("Updated {0} videos", totalCount);
                            count = 0;
                        }
                        else
                        {
                            videoList += youtubeId.Trim() + ",";
                        }

                        count++;
                    }



                    videoResponse = YoutubeApiHandler.FetchVideoById(videoList.TrimEnd(','), "snippet");

                    foreach (Video video in videoResponse.Items)
                    {
                        DateTime? publishedAt = video.Snippet.PublishedAt;

                        string udpateSql = string.Format("update videos set PublishedAt='{0}' where YoutubeID='{1}';", publishedAt.Value.ToString("yyyy-MM-dd HH:MM:ss").Trim(), video.Id.Trim());

                        NpgsqlCommand updateCommand = new NpgsqlCommand(udpateSql, conn);
                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected < 1)
                        {
                            throw new Exception("Video update didn't complete correctly.");
                        }
                    }

                    videoList = string.Empty;
                    totalCount += count;
                    Console.WriteLine("Updated {0} videos", totalCount);
                    count = 0;

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with video insert: " + e.Message);
            }
        }

        public static void BuildThumbnailCollage(string youtubeId)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DBHandler.DatabaseConnStr))
                {
                    conn.Open();

                    // We check if the same youtube channel id has already been inserted
                    string selectByChannelSql = "select v.title, v.thumbnail, v.publishedat, c.title from videos v inner join channels c on c.channelid=v.channelid where c.YoutubeID='{0}' order by publishedat;";

                    NpgsqlCommand selectCommand = new NpgsqlCommand(string.Format(selectByChannelSql, youtubeId), conn);
                    NpgsqlDataReader reader = selectCommand.ExecuteReader();

                    List<string> thumbnails = new List<string>();
                    string imgTagFormat = "<img src=\"{0}\" height=\"50\" width=\"80\">";

                    while (reader.Read())
                    {
                        string thumbnail = reader["Thumbnail"].ToString().Trim();
                        Console.WriteLine("Added " + thumbnail);
                        thumbnails.Add(string.Format(imgTagFormat, thumbnail));
                    }

                    reader.Close();


                    using (StreamWriter writer = new StreamWriter(@"C:\Users\Gabe\Desktop\test.html"))
                    {
                        foreach (string image in thumbnails)
                        {
                            writer.WriteLine(image);
                        }
                    }



                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with video insert: " + e.Message);
            }
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
            foreach(ObjectHolder apiResponse in allYoutubeChannelIds)
            {
                ChannelHolder channel = apiResponse as ChannelHolder;

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
                status = DBHandler.DoesItemExist("Videos", "YoutubeID", latestVideoId);
            }

            return status;
        }

        public static void FetchMissingChannelUploads(string youtubeChannelId)
        {
            // Check if completely new channel
            if (!DBHandler.DoesItemExist("Channels", "YoutubeID", youtubeChannelId))
            {
                // Channel doesn't exist. Throw an exception because this function will only
                // query for videos 5 at a time
                throw new Exception("FetchMissingChannelUploads(): Unrecognized youtube channel id: " + youtubeChannelId);
            }

            // Fetch actual channel id from youtube channel id
            int channelId = DBHandler.RetrieveIdFromYoutubeId("ChannelID", "Channels", youtubeChannelId);

            // Check if completely new channel with no videos written to database
            if (!DBHandler.DoesItemExist("Videos", "ChannelID", channelId))
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
                    PlaylistItemListResponse response = YoutubeApiHandler.FetchVideosByPlaylist(uploadPlaylistId, pageToken, "snippet", 5);
                    pageToken = response.NextPageToken;

                    foreach (PlaylistItem item in response.Items)
                    {
                        string youtubeVideoId = item.Snippet.ResourceId.VideoId;

                        // Check if video is in database
                        if (!DBHandler.DoesItemExist("Videos", "YoutubeID", youtubeVideoId))
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

        public static void FetchAllSubscriptionsToAllChannels()
        {
            int count = 1;
            ChannelHolder channel = null;

            // Get all channel youtube ids from database
            List<ObjectHolder> allYoutubeChannelIds = DBHandler.RetrieveColumnsFromTable(typeof(ChannelHolder), "YoutubeID,Title", "Channels");

            try
            {
                // API request 1 video
                foreach (ObjectHolder apiResponse in allYoutubeChannelIds)
                {
                    channel = apiResponse as ChannelHolder;

                    if (YoutubeApiHandler.DoesChannelHavePublicSubscriptions(channel.YoutubeId))
                    {
                        Console.WriteLine(count++ + ". Fetching subscriptions for " + channel.Title);
                        RecordChannelSubscriptions(channel.YoutubeId);
                    }
                    else
                    {
                        count++;
                    }
                }
            }
            catch(Exception)
            {
                Console.WriteLine(DateTime.Now + ": Program crashed on #{0}: {1}", count, channel.Title);

                using (StreamWriter writer = File.AppendText(YOUTUBE_LOG_FILE))
                {
                    writer.WriteLine(DateTime.Now + ": Program crashed on #{0}: {1}", count, channel.Title);
                }
            }
            

            // Check if channel has public subscriptions

            // Record subscriptions of channel to database
        }

        public static void InsertCollectionsData()
        {
            //CollectionHolder collection = new CollectionHolder();
            //collection.OwnerChannelId = 117;
            //collection.Title = "Funny";


            //CollectionItemHolder collectionItem = new CollectionItemHolder();
            //collectionItem.CollectionId = 2;
            //collectionItem.ItemChannelId = 76;

            // TODO: finish collection item insert functionality
            //DBHandler.InsertCollection(collection);
            DBHandler.InsertCollectionItem(3, -101);
        }

    }
}
