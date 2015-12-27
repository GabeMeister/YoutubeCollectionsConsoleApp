using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCollections.Database.YoutubeObjects
{
    public class Channel
    {
        public string YoutubeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UploadPlaylist { get; set; }
        public string Thumbnail { get; set; }
        public string ViewCount { get; set; }
        public string SubscriberCount { get; set; }
        public string VideoCount { get; set; }
    }
}
