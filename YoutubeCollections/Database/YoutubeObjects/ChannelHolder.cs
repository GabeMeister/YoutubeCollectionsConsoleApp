using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCollections.Database.YoutubeObjects
{
    public class ChannelHolder
    {
        public string YoutubeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UploadPlaylist { get; set; }
        public string Thumbnail { get; set; }
        public ulong? ViewCount { get; set; }
        public ulong? SubscriberCount { get; set; }
        public ulong? VideoCount { get; set; }

        public string FetchInsertSql()
        {
            return string.Format(SqlConst.InsertChannelSql, DBUtil.Sanitize(YoutubeId), DBUtil.Sanitize(Title),
                        DBUtil.Sanitize(Description), DBUtil.Sanitize(UploadPlaylist), DBUtil.Sanitize(Thumbnail), DBUtil.Sanitize(ViewCount), DBUtil.Sanitize(SubscriberCount),
                        DBUtil.Sanitize(VideoCount));
        }
    }
}
