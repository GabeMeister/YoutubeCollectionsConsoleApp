using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeCollections.Api.ApiResponseHolders;

namespace YoutubeCollections.Database
{
    public class SqlBuilder
    {
        // ============================ GENERAL
        # region GENERAL
        public static string SelectByIdSql(string columnsToSelect, string table, string columnToQueryFor, string id)
        {
            return string.Format(@"select {0} from {1} where {2}='{3}';", Sanitize(columnsToSelect), Sanitize(table), Sanitize(columnToQueryFor), Sanitize(id));
        }

        public static string SelectByIdSql(string columnsToSelect, string table, string columnToQueryFor, int id)
        {
            return string.Format(@"select {0} from {1} where {2}={3};", Sanitize(columnsToSelect), Sanitize(table), Sanitize(columnToQueryFor), Sanitize(id));
        }

        public static string SelectAllSql(string columns, string table)
        {
            return string.Format(@"select {0} from {1};", Sanitize(columns), Sanitize(table));
        }


        #endregion

        // ============================ CHANNELS
        #region CHANNELS
        public static string FetchSelectChannelByChannelIdSql(string columns, ulong? channelId)
        {
            return string.Format(@"select {0} from Channels where ChannelID={1};", Sanitize(columns), Sanitize(channelId));
        }

        public static string FetchInsertChannelSql(ChannelHolder channel)
        {
            return string.Format(@"insert into Channels (YoutubeID,Title,Description,UploadPlaylist,Thumbnail,ViewCount,SubscriberCount,VideoCount) values ('{0}','{1}','{2}','{3}','{4}',{5},{6},{7});",
                Sanitize(channel.YoutubeId),
                Sanitize(channel.Title),
                Sanitize(channel.Description),
                Sanitize(channel.UploadPlaylist),
                Sanitize(channel.Thumbnail),
                Sanitize(channel.ViewCount),
                Sanitize(channel.SubscriberCount),
                Sanitize(channel.VideoCount));
        }


        #endregion

        // ============================ SUBSCRIPTIONS
        #region SUBSCRIPTIONS
        public static string SelectBySubscriberIdsSql(string columns, int subscriberChannelId, int beingSubscribedToChannelId)
        {
            return string.Format(@"select {0} from Subscriptions where SubscriberChannelID='{1}' and BeingSubscribedToChannelID='{2}';", 
                Sanitize(columns), 
                Sanitize(subscriberChannelId),
                Sanitize(beingSubscribedToChannelId));
        }

        public static string InsertSubscriptionByChannelIdSql(int subscriberChannelId, int beingSubscribedToChannelId)
        {
            return string.Format(@"insert into Subscriptions (SubscriberChannelID, BeingSubscribedToChannelID) values ({0}, {1});",
                Sanitize(subscriberChannelId),
                Sanitize(beingSubscribedToChannelId));
        }


        #endregion

        // ============================ VIDEOS
        #region VIDEOS
        public static string SelectVideoByVideoIdSql(string columns, ulong? videoId)
        {
            return string.Format(@"select {0} from Videos where VideoID={1};", Sanitize(columns), Sanitize(videoId));
        }

        public static string InsertVideoSql(VideoHolder video)
        {
            return string.Format(@"insert into Videos (YoutubeID,ChannelID,Title,Thumbnail,Duration,ViewCount,PublishedAt) values ('{0}',{1},'{2}','{3}','{4}',{5},'{6}');",
                Sanitize(video.YoutubeId),
                Sanitize(video.ChannelId),
                Sanitize(video.Title),
                Sanitize(video.Thumbnail),
                Sanitize(video.Duration),
                Sanitize(video.ViewCount),
                Sanitize(video.PublishedAt.Value.ToString("yyyy-MM-dd HH:MM:ss")));
        }


        #endregion




        #region UTILITY FUNCTIONS
        private static string Sanitize(object str)
        {
            return str.ToString().Replace("'", "''").Trim();
        }


        #endregion
    }
}
