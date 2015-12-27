using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCollections.Database
{
    public class SqlConst
    {
        public static string DatabaseConnStr = @"Server=127.0.0.1;Port=5432;User Id=postgres;Password=4321;Database=YoutubeCollections";
        public static string InsertChannelSql = @"insert into Channels (YoutubeID,Title,Description,UploadPlaylist,Thumbnail,ViewCount,SubscriberCount,VideoCount) values ('{0}','{1}','{2}','{3}','{4}',{5},{6},{7});";


    }
}
