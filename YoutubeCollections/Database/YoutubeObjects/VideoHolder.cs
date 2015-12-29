using Google.Apis.YouTube.v3.Data;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace YoutubeCollections.Database.YoutubeObjects
{
    public class VideoHolder : YoutubeObjectHolder
    {
        public ulong? VideoHolderId { get; set; }
        public string YoutubeId { get; set; }
        public ulong? ChannelId { get; set; }
        public string YoutubeChannelId { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string Duration { get; set; }
        public ulong? ViewCount { get; set; }
        public DateTime? PublishedAt { get; set; }


        public VideoHolder()
        {
            VideoHolderId = 0;
            YoutubeId = string.Empty;
            ChannelId = 0;
            Title = string.Empty;
            Thumbnail = string.Empty;
            Duration = string.Empty;
            ViewCount = 0;
            PublishedAt = null;
        }

        public VideoHolder(string youtubeId)
        {
            YoutubeId = youtubeId;

            try
            {
                using (var conn = new NpgsqlConnection(SqlConst.DatabaseConnStr))
                {
                    conn.Open();

                    // We check if the same youtube channel id has already been inserted
                    string selectSql = FetchSelectByYoutubeIdSql();
                    NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                    NpgsqlDataReader reader = selectCommand.ExecuteReader();


                    if (reader.Read())
                    {
                        InitializeFromDatabase(reader);
                    }
                    else
                    {
                        throw new Exception("Couldn't find correct video by youtube id");
                    }

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with video querying: " + e.Message);
            }
        }

        public VideoHolder(ulong? videoId)
        {
            VideoHolderId = videoId;

            try
            {
                using (var conn = new NpgsqlConnection(SqlConst.DatabaseConnStr))
                {
                    conn.Open();

                    // We check if the same youtube channel id has already been inserted
                    string selectSql = FetchSelectByVideoIdSql();
                    NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                    NpgsqlDataReader reader = selectCommand.ExecuteReader();


                    if (reader.Read())
                    {
                        InitializeFromDatabase(reader);
                    }
                    else
                    {
                        throw new Exception("Couldn't find correct video by video id");
                    }

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with video querying: " + e.Message);
            }
        }

        public VideoHolder(Video videoResponse)
        {
            // Can't assign the actual id because we are populating from an API response
            VideoHolderId = 0;
            YoutubeId = videoResponse.Id.ToString().Trim();
            // Can't assign the actual channel id because we are populating from an API response
            ChannelId = 0;
            YoutubeChannelId = videoResponse.Snippet.ChannelId.ToString().Trim();
            Title = videoResponse.Snippet.Title.ToString().Trim();
            Thumbnail = videoResponse.Snippet.Thumbnails.Medium.Url.ToString().Trim();
            // We don't want the ISO format, "PT2m34s". We want the Timespan format: "00:02:34"
            Duration = XmlConvert.ToTimeSpan(videoResponse.ContentDetails.Duration).ToString().Trim();
            ViewCount = videoResponse.Statistics.ViewCount;
            PublishedAt = videoResponse.Snippet.PublishedAt;
        }



        public int InsertVideo()
        {
            int rowsAffected = 0;

            try
            {
                using (var conn = new NpgsqlConnection(SqlConst.DatabaseConnStr))
                {
                    conn.Open();

                    // We check if the same youtube channel id has already been inserted
                    string selectSql = FetchSelectByYoutubeIdSql();
                    NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                    NpgsqlDataReader reader = selectCommand.ExecuteReader();
                    bool alreadyExists = reader.HasRows;
                    reader.Close();

                    if (!alreadyExists)
                    {
                        // Have to get the actual channel id first
                        ChannelHolder channel = new ChannelHolder(YoutubeChannelId);
                        ChannelId = channel.ChannelHolderId;

                        // We actually insert the video because we know it's not in the database
                        string insertSQL = FetchInsertSql();
                        NpgsqlCommand insertCommand = new NpgsqlCommand(insertSQL, conn);

                        rowsAffected = insertCommand.ExecuteNonQuery();

                        if (rowsAffected < 1)
                        {
                            throw new Exception("Video insert didn't complete correctly.");
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with video insert: " + e.Message);
            }

            return rowsAffected;
        }

        public override string FetchInsertSql()
        {
            return string.Format(SqlConst.InsertVideoSql, DBUtil.Sanitize(YoutubeId), DBUtil.Sanitize(ChannelId), DBUtil.Sanitize(Title),
                DBUtil.Sanitize(Thumbnail), DBUtil.Sanitize(Duration), DBUtil.Sanitize(ViewCount), DBUtil.Sanitize(PublishedAt.Value.ToString("yyyy-MM-dd HH:MM:ss")));
        }

        public string FetchSelectByVideoIdSql()
        {
            return string.Format(SqlConst.SelectVideoByVideoIdSql, DBUtil.Sanitize(VideoHolderId));
        }

        public string FetchSelectByYoutubeIdSql()
        {
            return string.Format(SqlConst.SelectVideoByYoutubeIdSql, DBUtil.Sanitize(YoutubeId));
        }


        protected override void InitializeFromDatabase(NpgsqlDataReader reader)
        {
            VideoHolderId = Convert.ToUInt64(reader["VideoID"].ToString().Trim());
            YoutubeId = reader["YoutubeID"].ToString().Trim();
            ChannelId = Convert.ToUInt64(reader["ChannelId"].ToString().Trim());
            Title = reader["Title"].ToString().Trim();
            Thumbnail = reader["Thumbnail"].ToString().Trim();
            Duration = reader["Duration"].ToString().Trim();
            ViewCount = Convert.ToUInt64(reader["ViewCount"].ToString().Trim());
            PublishedAt = DateTime.Parse(reader["PublishedAt"].ToString().Trim());
        }
        
    }
}
