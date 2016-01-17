using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YoutubeCollections.Api.ApiResponseHolders;
using YoutubeCollections.Database;

namespace YoutubeCollections.Api
{
    public class DBHandler
    {
        public static string DatabaseConnStr = @"Server=127.0.0.1;Port=5432;User Id=postgres;Password=4321;Database=YoutubeCollections";

        // ============================ GENERAL
        #region GENERAL
        public static int RetrieveIdFromYoutubeId(string idColumnToSelect, string table, string youtubeId)
        {
            int id = -1;

            using (NpgsqlConnection conn = new NpgsqlConnection(DatabaseConnStr))
            {
                conn.Open();

                string selectSql = SqlBuilder.SelectByIdSql(idColumnToSelect, table, "YoutubeID", youtubeId);
                NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                NpgsqlDataReader reader = selectCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    id = Convert.ToInt32(reader[idColumnToSelect].ToString().Trim());
                }

                conn.Close();
            }

            return id;
        }

        public static string RetrieveColumnBySingleCondition(string columnToSelect, string table, string columnToQuery, string queryValue)
        {
            string value = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(DatabaseConnStr))
            {
                conn.Open();

                string selectSql = SqlBuilder.SelectByIdSql(columnToSelect, table, columnToQuery, queryValue);
                NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                NpgsqlDataReader reader = selectCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    value = reader[columnToSelect].ToString().Trim();
                }

                conn.Close();
            }

            return value;
        }

        public static List<string> RetrieveColumnFromTable(string columnToSelect, string table)
        {
            List<string> youtubeIds = new List<string>();

            using (NpgsqlConnection conn = new NpgsqlConnection(DatabaseConnStr))
            {
                conn.Open();

                string selectSql = SqlBuilder.SelectAllSql(columnToSelect, table);
                NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                NpgsqlDataReader reader = selectCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string youtubeId = reader[columnToSelect].ToString().Trim();
                        youtubeIds.Add(youtubeId);
                    }
                }

                conn.Close();
            }

            return youtubeIds;
        }

        public static List<ApiResponseHolder> RetrieveColumnsFromTable(Type itemType, string columnsToSelect, string table)
        {
            List<ApiResponseHolder> items = new List<ApiResponseHolder>();

            using (NpgsqlConnection conn = new NpgsqlConnection(DatabaseConnStr))
            {
                conn.Open();

                // TODO: change back to no offset
                string selectSql = SqlBuilder.SelectAllSql(columnsToSelect, table).Replace(";", " offset 143;");
                NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                NpgsqlDataReader reader = selectCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ConstructorInfo constructor = itemType.GetConstructor(new[] { typeof(NpgsqlDataReader) });
                        ApiResponseHolder newItem = constructor.Invoke(new object[] { reader }) as ApiResponseHolder;

                        items.Add(newItem);
                    }
                }

                conn.Close();
            }

            return items;
        }

        #endregion

        // ============================ JOINS
        #region JOINS

        #endregion

        // ============================ CHANNELS
        #region CHANNELS
        public static int InsertChannel(ChannelHolder channel)
        {
            int rowsAffected = 0;

            // We check if the same youtube channel id has already been inserted
            if (!DoesItemExist("Channels", "YoutubeID", channel.YoutubeId))
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(DatabaseConnStr))
                {
                    conn.Open();

                    // Now we actually insert the channel because we know it's not in the database
                    string insertSQL = SqlBuilder.FetchInsertChannelSql(channel);
                    NpgsqlCommand insertCommand = new NpgsqlCommand(insertSQL, conn);
                    rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected < 1)
                    {
                        throw new Exception("Channel insert didn't complete correctly.");
                    }

                    conn.Close();
                }
            }

            return rowsAffected;
        }

        

        #endregion

        // ============================ SUBSCRIPTIONS
        #region SUBSCRIPTIONS
        public static int InsertSubscription(int subscriberChannelId, int beingSubscribedToChannelId)
        {
            int rowsAffected = 0;

            // We check if the subscription already exists.
            if (!DoesSubscriptionExist(subscriberChannelId, beingSubscribedToChannelId))
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(DatabaseConnStr))
                {
                    conn.Open();

                    // Now we actually insert the channel because we know it's not in the database
                    string insertSQL = SqlBuilder.InsertSubscriptionByChannelIdSql(subscriberChannelId, beingSubscribedToChannelId);
                    NpgsqlCommand insertCommand = new NpgsqlCommand(insertSQL, conn);
                    rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected < 1)
                    {
                        throw new Exception("Subscription insert didn't complete correctly.");
                    }

                    conn.Close();
                }
            }

            return rowsAffected;
        }

        
        #endregion

        // ============================ VIDEOS
        #region VIDEOS
        public static int InsertVideo(VideoHolder video)
        {
            int rowsAffected = 0;

            // We check if the same youtube channel id has already been inserted
            bool alreadyExists = DoesItemExist("Videos", "YoutubeID", video.YoutubeId);

            if (!alreadyExists)
            {
                // We actually insert the video because we know it's not in the database
                using (NpgsqlConnection conn = new NpgsqlConnection(DatabaseConnStr))
                {
                    conn.Open();

                    // Have to get the actual channel id first, from the youtube id
                    string selectSql = SqlBuilder.SelectByIdSql("ChannelID", "Channels", "YoutubeID", video.YoutubeChannelId);
                    NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                    NpgsqlDataReader reader = selectCommand.ExecuteReader();
                    reader.Read();
                    video.ChannelId = Convert.ToUInt64(reader["ChannelID"].ToString().Trim());
                    reader.Close();

                    string insertSQL = SqlBuilder.InsertVideoSql(video);
                    NpgsqlCommand insertCommand = new NpgsqlCommand(insertSQL, conn);
                    rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected < 1)
                    {
                        throw new Exception("Video insert didn't complete correctly.");
                    }

                    conn.Close();
                }
            }

            return rowsAffected;
        }


        #endregion




        #region Utilities
        public static bool DoesItemExist(string table, string columnToQuery, string id)
        {
            bool doesExist = false;

            using (NpgsqlConnection conn = new NpgsqlConnection(DatabaseConnStr))
            {
                conn.Open();

                string selectSql = SqlBuilder.SelectByIdSql("count(*)", table, columnToQuery, id);
                NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                int count = Convert.ToInt16(selectCommand.ExecuteScalar());

                if (count > 0)
                {
                    doesExist = true;
                }

                conn.Close();
            }

            return doesExist;
        }

        public static bool DoesItemExist(string table, string columnToQuery, int id)
        {
            bool doesExist = false;

            using (NpgsqlConnection conn = new NpgsqlConnection(DatabaseConnStr))
            {
                conn.Open();

                string selectSql = SqlBuilder.SelectByIdSql("count(*)", table, columnToQuery, id);
                NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                int count = Convert.ToInt16(selectCommand.ExecuteScalar());

                if (count > 0)
                {
                    doesExist = true;
                }

                conn.Close();
            }

            return doesExist;
        }

        public static bool DoesSubscriptionExist(int subscriberChannelId, int beingSubscribedToChannelId)
        {
            bool doesExist = false;

            using (NpgsqlConnection conn = new NpgsqlConnection(DatabaseConnStr))
            {
                conn.Open();

                string selectSql = SqlBuilder.SelectBySubscriberIdsSql("count(*)", subscriberChannelId, beingSubscribedToChannelId);
                NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, conn);
                int count = Convert.ToInt16(selectCommand.ExecuteScalar());

                if (count > 0)
                {
                    doesExist = true;
                }

                conn.Close();
            }

            return doesExist;
        }

        
        #endregion

    }
}
