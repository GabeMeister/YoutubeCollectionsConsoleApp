using Google.Apis.YouTube.v3.Data;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCollections.Database.YoutubeObjects
{
    public class ChannelHolder : YoutubeObjectHolder
    {
        public ulong? ChannelHolderId { get; set; }
        public string YoutubeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UploadPlaylist { get; set; }
        public string Thumbnail { get; set; }
        public ulong? ViewCount { get; set; }
        public ulong? SubscriberCount { get; set; }
        public ulong? VideoCount { get; set; }

        


        public ChannelHolder()
        {
            ChannelHolderId = 0;
            YoutubeId = string.Empty;
            Title = string.Empty;
            Description = string.Empty;
            UploadPlaylist = string.Empty;
            Thumbnail = string.Empty;
            ViewCount = 0;
            SubscriberCount = 0;
            VideoCount = 0;
        }

        public ChannelHolder(string youtubeId)
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
                        ChannelHolderId = Convert.ToUInt64(reader["ChannelID"]);
                        YoutubeId = reader["YoutubeID"].ToString();
                        Title = reader["Title"].ToString();
                        Description = reader["Description"].ToString();
                        UploadPlaylist = reader["UploadPlaylist"].ToString();
                        Thumbnail = reader["Thumbnail"].ToString();
                        ViewCount = Convert.ToUInt64(reader["ViewCount"]);
                        SubscriberCount = Convert.ToUInt64(reader["SubscriberCount"]);
                        VideoCount = Convert.ToUInt64(reader["VideoCount"]);
                    }
                    else
                    {
                        throw new Exception("Couldn't find correct channel by youtube id");
                    }

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with channel querying: " + e.Message);
            }
            
        }

        public ChannelHolder(ulong? channelId)
        {
            ChannelHolderId = channelId;

            try
            {
                using (var conn = new NpgsqlConnection(SqlConst.DatabaseConnStr))
                {
                    conn.Open();

                    // We check if the same youtube channel id has already been inserted
                    string selectSql = FetchSelectByChannelIdSql();
                    NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                    NpgsqlDataReader reader = selectCommand.ExecuteReader();


                    if (reader.Read())
                    {
                        ChannelHolderId = Convert.ToUInt64(reader["ChannelID"]);
                        YoutubeId = reader["YoutubeID"].ToString();
                        Title = reader["Title"].ToString();
                        Description = reader["Description"].ToString();
                        UploadPlaylist = reader["UploadPlaylist"].ToString();
                        Thumbnail = reader["Thumbnail"].ToString();
                        ViewCount = Convert.ToUInt64(reader["ViewCount"]);
                        SubscriberCount = Convert.ToUInt64(reader["SubscriberCount"]);
                        VideoCount = Convert.ToUInt64(reader["VideoCount"]);
                    }
                    else
                    {
                        throw new Exception("Couldn't find correct channel by channel id");
                    }

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with channel querying: " + e.Message);
            }

        }

        public ChannelHolder(Channel channelResponse)
        {
            // Channel holder id is empty because we are populating from an API call
            ChannelHolderId = 0;
            YoutubeId = channelResponse.Id;
            Title = channelResponse.Snippet.Title;
            Description = channelResponse.Snippet.Description;
            UploadPlaylist = channelResponse.ContentDetails.RelatedPlaylists.Uploads;
            Thumbnail = channelResponse.Snippet.Thumbnails.Medium.Url;
            ViewCount = channelResponse.Statistics.ViewCount;
            SubscriberCount = channelResponse.Statistics.SubscriberCount;
            VideoCount = channelResponse.Statistics.VideoCount;
        }

        public int InsertChannel()
        {
            int rowsAffected = 0;

            try
            {
                using (var conn = new NpgsqlConnection(SqlConst.DatabaseConnStr))
                {
                    conn.Open();

                    // We check if the same youtube channel id has already been inserted
                    string selectSQL = FetchSelectByYoutubeIdSql();
                    NpgsqlCommand selectCommand = new NpgsqlCommand(selectSQL, conn);
                    NpgsqlDataReader reader = selectCommand.ExecuteReader();
                    bool alreadyExists = reader.HasRows;
                    reader.Close();


                    if (!alreadyExists)
                    {
                        // We actually insert the channel because we know it's not in the database
                        string insertSQL = FetchInsertSql();
                        NpgsqlCommand insertCommand = new NpgsqlCommand(insertSQL, conn);

                        rowsAffected = insertCommand.ExecuteNonQuery();

                        if (rowsAffected < 1)
                        {
                            throw new Exception("Channel insert didn't complete correctly.");
                        }

                    }


                    // Assign the actual channel id now that it's inserted
                    selectSQL = FetchSelectByYoutubeIdSql();
                    selectCommand = new NpgsqlCommand(selectSQL, conn);
                    reader = selectCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        ChannelHolderId = Convert.ToUInt64(reader["ChannelId"]);
                    }
                    reader.Close();


                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with channel insert: " + e.Message);
            }

            return rowsAffected;
        }

        public override string FetchInsertSql()
        {
            return string.Format(SqlConst.InsertChannelSql, DBUtil.Sanitize(YoutubeId), DBUtil.Sanitize(Title),
                        DBUtil.Sanitize(Description), DBUtil.Sanitize(UploadPlaylist), DBUtil.Sanitize(Thumbnail), DBUtil.Sanitize(ViewCount), DBUtil.Sanitize(SubscriberCount),
                        DBUtil.Sanitize(VideoCount));
        }

        protected string FetchSelectByChannelIdSql()
        {
            return string.Format(SqlConst.SelectChannelByChannelIdSql, DBUtil.Sanitize(ChannelHolderId));
        }

        protected string FetchSelectByYoutubeIdSql()
        {
            return string.Format(SqlConst.SelectChannelByYoutubeIdSql, DBUtil.Sanitize(YoutubeId));
        }

        
    }
}
