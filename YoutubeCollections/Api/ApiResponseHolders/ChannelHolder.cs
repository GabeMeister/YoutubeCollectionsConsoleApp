using Google.Apis.YouTube.v3.Data;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCollections.Api.ApiResponseHolders
{
    public class ChannelHolder
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
            ChannelHolderId = null;
            YoutubeId = string.Empty;
            Title = string.Empty;
            Description = string.Empty;
            UploadPlaylist = string.Empty;
            Thumbnail = string.Empty;
            ViewCount = 0;
            SubscriberCount = 0;
            VideoCount = 0;
        }

        public ChannelHolder(Channel channelResponse)
        {
            // Channel holder id is empty because we are populating from an API call
            ChannelHolderId = null;
            YoutubeId = channelResponse.Id.ToString().Trim();
            Title = channelResponse.Snippet.Title.ToString().Trim();
            Description = channelResponse.Snippet.Description.ToString().Trim();
            UploadPlaylist = channelResponse.ContentDetails.RelatedPlaylists.Uploads.ToString().Trim();
            Thumbnail = channelResponse.Snippet.Thumbnails.Medium.Url.ToString().Trim();
            ViewCount = channelResponse.Statistics.ViewCount;
            SubscriberCount = channelResponse.Statistics.SubscriberCount;
            VideoCount = channelResponse.Statistics.VideoCount;
        }


        protected void InitializeFromDatabase(NpgsqlDataReader reader)
        {
            // TODO: refactor this to include checks for each attribute before initializing it.
            ChannelHolderId = Convert.ToUInt64(reader["ChannelID"].ToString().Trim());
            YoutubeId = reader["YoutubeID"].ToString().Trim();
            Title = reader["Title"].ToString().Trim();
            Description = reader["Description"].ToString().Trim();
            UploadPlaylist = reader["UploadPlaylist"].ToString().Trim();
            Thumbnail = reader["Thumbnail"].ToString().Trim();
            ViewCount = Convert.ToUInt64(reader["ViewCount"].ToString().Trim());
            SubscriberCount = Convert.ToUInt64(reader["SubscriberCount"].ToString().Trim());
            VideoCount = Convert.ToUInt64(reader["VideoCount"].ToString().Trim());
        }

        

        
    }
}
