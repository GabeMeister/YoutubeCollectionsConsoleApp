using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Xml;
using YoutubeCollections.Database;
using YoutubeCollections.Database.YoutubeObjects;
using System.IO;
using Npgsql;

namespace YoutubeCollections
{
    public class YoutubeWrapper
    {
        private const int MAX_RESULTS = 50;

        public static void FetchChannelSubscriptions(string channelId)
        {
            // NOTE: cannot view other channel subscriptions

            int subscriptionCount = 0;
            string nextPageToken = string.Empty;
            SubscriptionListResponse subscriptionsList;

            do
            {
                subscriptionsList = YoutubeApiHandler.FetchSubscriptionsByChannel(channelId, nextPageToken, "snippet");
                subscriptionCount += subscriptionsList.Items.Count;
                nextPageToken = subscriptionsList.NextPageToken;

                if (subscriptionsList != null)
                {
                    foreach (var searchResult in subscriptionsList.Items)
                    {
                        FetchChannelUploads(searchResult.Snippet.ResourceId.ChannelId);
                    }
                }
            }
            while (nextPageToken != null);

            Console.WriteLine("Total Subscription Count: " + subscriptionCount);
        }

        public static void FetchChannelUploads(string channelId)
        {
            int vidCount = 0;
            ChannelListResponse channelResponse = YoutubeApiHandler.FetchUploadsPlaylistByChannel(channelId, "snippet,contentDetails,statistics");
            ChannelHolder channel = new ChannelHolder(channelResponse.Items[0]);
            channel.InsertChannel();

            Console.WriteLine("************* " + channel.Title + " | " + channel.YoutubeId + " *************");

            string nextPageToken = string.Empty;
            string uploadsPlaylistId = channelResponse.Items[0].ContentDetails.RelatedPlaylists.Uploads;
            PlaylistItemListResponse searchListResponse;

            do
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
            while (nextPageToken != null);

            Console.WriteLine("Total Video Count: " + vidCount);

        }

        public static void FetchVideoInfo(string videoIds)
        {
            VideoListResponse videos = YoutubeApiHandler.FetchVideoById(videoIds, "snippet,contentDetails,statistics");

            foreach(var videoResponse in videos.Items)
            {
                VideoHolder video = new VideoHolder(videoResponse);
                video.InsertVideo();

                Console.WriteLine("====================");
                Console.WriteLine(video.Title);
                Console.WriteLine(video.Thumbnail);
                Console.WriteLine(video.Duration);
                Console.WriteLine(video.ViewCount);
            }
            
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
                    string channelName = tokens[0];
                    string channelId = tokens[1];

                    FetchChannelUploads(channelId);

                }
            }
        }

        public static void AddPublishedAtTimeStamps()
        {
            try
            {
                using (var conn = new NpgsqlConnection(SqlConst.DatabaseConnStr))
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

        public static void BuildThumbnailCollage()
        {
            try
            {
                using (var conn = new NpgsqlConnection(SqlConst.DatabaseConnStr))
                {
                    conn.Open();

                    // We check if the same youtube channel id has already been inserted
                    string channelToDisplay = "Brothers Green Eats";
                    string selectByChannelSql = "select v.title, v.thumbnail, v.publishedat, c.title from videos v inner join channels c on c.channelid=v.channelid where c.title='{0}' order by publishedat;";
                    string oldVideoSelectSql = "select * from videos order by publishedat limit 200;";
                    string newVideoSelectSql = "select * from videos order by publishedat desc limit 200;";

                    NpgsqlCommand selectCommand = new NpgsqlCommand(string.Format(selectByChannelSql, channelToDisplay), conn);
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

    }
}
