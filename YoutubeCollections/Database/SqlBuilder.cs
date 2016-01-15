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
        public static string FetchSelectByYoutubeIdSql(string columns, string table, string youtubeId)
        {
            return string.Format(@"select {0} from {1} where YoutubeID='{1}';", Sanitize(columns), Sanitize(table), Sanitize(youtubeId));
        }

        public static string FetchSelectAllSql(string columns, string table)
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
        public static string FetchSelectBySubscriberIdSql(string columns, string subscriberChannelId, string beingSubscribedToChannelId)
        {
            return string.Format(@"select {0} from Channels where SubscriberChannelId='{1}';", Sanitize(columns), Sanitize(subscriberChannelId));
        }

        public static string FetchInsertSubscriptionByChannelIdSql(int subscriberChannelId, int beingSubscribedToChannelId)
        {
            return string.Format(@"insert into Subscriptions (SubscriberChannelID, BeingSubscribedToChannelID) values ({0}, {1});",
                Sanitize(subscriberChannelId),
                Sanitize(beingSubscribedToChannelId));
        }


        #endregion

        // ============================ VIDEOS
        #region VIDEOS
        public static string FetchSelectVideoByVideoIdSql(string columns, ulong? videoId)
        {
            return string.Format(@"select {0} from Videos where VideoID={1};", Sanitize(columns), Sanitize(videoId));
        }

        public static string FetchInsertVideoSql(VideoHolder video)
        {
            return string.Format(@"insert into Videos (YoutubeID,ChannelID,Title,Thumbnail,Duration,ViewCount,PublishedAt) values ('{0}',{1},'{2}','{3}','{4}',{5},'{6}');",
                Sanitize(video.YoutubeId),
                Sanitize(video.ChannelId),
                Sanitize(video.Title),
                Sanitize(video.Thumbnail),
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
