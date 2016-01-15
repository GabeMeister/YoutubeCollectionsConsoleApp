using Google.Apis.YouTube.v3.Data;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace YoutubeCollections.Api.ApiResponseHolders
{
    public class VideoHolder
    {
        public ulong? VideoHolderId { get; set; }
        public string YoutubeId { get; set; }
        public ulong? ChannelId { get; set; }
        public string YoutubeChannelId { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string Duration { get; set; }
        public ulong? ViewCount { get; set; }
        public DateTime? PublishedAt { get; set; }


        public VideoHolder()
        {
            VideoHolderId = 0;
            YoutubeId = string.Empty;
            ChannelId = 0;
            Title = string.Empty;
            Thumbnail = string.Empty;
            Duration = string.Empty;
            ViewCount = 0;
            PublishedAt = null;
        }

        public VideoHolder(Video videoResponse)
        {
            // Can't assign the actual id because we are populating from an API response
            VideoHolderId = 0;
            YoutubeId = videoResponse.Id.ToString().Trim();
            // Can't assign the actual channel id because we are populating from an API response
            ChannelId = 0;
            YoutubeChannelId = videoResponse.Snippet.ChannelId.ToString().Trim();
            Title = videoResponse.Snippet.Title.ToString().Trim();
            Thumbnail = videoResponse.Snippet.Thumbnails.Medium.Url.ToString().Trim();
            // We don't want the ISO format, "PT2m34s". We want the Timespan format: "00:02:34"
            Duration = XmlConvert.ToTimeSpan(videoResponse.ContentDetails.Duration).ToString().Trim();
            ViewCount = videoResponse.Statistics.ViewCount;
            PublishedAt = videoResponse.Snippet.PublishedAt;
        }


        protected void InitializeFromDatabase(NpgsqlDataReader reader)
        {
            // TODO: refactor this to include checks for each attribute before initializing it.
            VideoHolderId = Convert.ToUInt64(reader["VideoID"].ToString().Trim());
            YoutubeId = reader["YoutubeID"].ToString().Trim();
            ChannelId = Convert.ToUInt64(reader["ChannelId"].ToString().Trim());
            Title = reader["Title"].ToString().Trim();
            Thumbnail = reader["Thumbnail"].ToString().Trim();
            Duration = reader["Duration"].ToString().Trim();
            ViewCount = Convert.ToUInt64(reader["ViewCount"].ToString().Trim());
            PublishedAt = DateTime.Parse(reader["PublishedAt"].ToString().Trim());
        }
        
    }
}
