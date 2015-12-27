using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeCollections.Database;
using YoutubeCollections.Database.YoutubeObjects;

namespace YoutubeCollections
{
    public class YoutubeDatabaseHandler
    {
        

        public static int InsertChannel(ChannelHolder channel)
        {
            int rowsAffected = -1;

            try
            {
                using (var conn = new NpgsqlConnection(SqlConst.DatabaseConnStr))
                {
                    conn.Open();
                    string insertSQL = channel.FetchInsertSql();
                    NpgsqlCommand command = new NpgsqlCommand(insertSQL, conn);

                    rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected < 1)
                    {
                        throw new Exception("Channel insert didn't complete correctly.");
                    }

                    conn.Close();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error with channel insert: " + e.Message);
            }
            

            return rowsAffected;
            
        }
    }
}
