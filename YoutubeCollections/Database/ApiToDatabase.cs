using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeCollections.Database.YoutubeObjects;

namespace YoutubeCollections.Database
{
    public class ApiToDatabase
    {
        public static ChannelHolder ConvertToChannelHolder(ChannelListResponse channelResponse)
        {
            return new ChannelHolder()
            {
                YoutubeId = channelResponse.Items[0].Id,
                Title = channelResponse.Items[0].Snippet.Title,
                Description = channelResponse.Items[0].Snippet.Description,
                UploadPlaylist = channelResponse.Items[0].ContentDetails.RelatedPlaylists.Uploads,
                Thumbnail = channelResponse.Items[0].Snippet.Thumbnails.Medium.Url,
                ViewCount = channelResponse.Items[0].Statistics.ViewCount,
                SubscriberCount = channelResponse.Items[0].Statistics.SubscriberCount,
                VideoCount = channelResponse.Items[0].Statistics.VideoCount
            };

        }
    }
}
