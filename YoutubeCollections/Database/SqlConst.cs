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
        public static string InsertVideoSql = @"insert into Videos (YoutubeID,ChannelID,Title,Thumbnail,Duration,ViewCount) values ('{0}',{1},'{2}','{3}','{4}',{5});";
        public static string SelectChannelByChannelIdSql = @"select * from Channels where ChannelID={0};";
        public static string SelectChannelByYoutubeIdSql = @"select * from Channels where YoutubeID='{0}';";
        public static string SelectVideoByVideoIdSql = @"select * from Videos where VideoID={0};";
        public static string SelectVideoByYoutubeIdSql = @"select * from Videos where YoutubeID='{0}';";
        



    }
}
