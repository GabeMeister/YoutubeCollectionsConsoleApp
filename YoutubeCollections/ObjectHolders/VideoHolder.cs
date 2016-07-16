﻿using Google.Apis.YouTube.v3.Data;
using Npgsql;
using System;
using System.Xml;

namespace YoutubeCollections.ObjectHolders
{
    public class VideoHolder : ObjectHolder
    {
        public int VideoHolderId { get; set; }
        public string YoutubeId { get; set; }
        public int ChannelId { get; set; }
        public string YoutubeChannelId { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string Duration { get; set; }
        public ulong? ViewCount { get; set; }
        public DateTime? PublishedAt { get; set; }


        public VideoHolder()
        {
            VideoHolderId = 0;
            YoutubeId = "";
            ChannelId = 0;
            Title = "";
            Thumbnail = "";
            Duration = "";
            ViewCount = 0;
            PublishedAt = null;
        }

        public VideoHolder(Video videoResponse)
        {
            // Can't assign the actual id because we are populating from an API response
            VideoHolderId = 0;
            YoutubeId = videoResponse.Id.Trim();
            // Can't assign the actual channel id because we are populating from an API response
            ChannelId = 0;
            YoutubeChannelId = videoResponse.Snippet.ChannelId.Trim();
            Title = videoResponse.Snippet.Title.Trim();
            Thumbnail = videoResponse.Snippet.Thumbnails.Medium.Url.Trim();
            // We don't want the ISO format, "PT2m34s". We want the Timespan format: "00:02:34"
            Duration = XmlConvert.ToTimeSpan(videoResponse.ContentDetails.Duration).ToString().Trim();
            ViewCount = videoResponse.Statistics.ViewCount;
            PublishedAt = videoResponse.Snippet.PublishedAt;
        }


        public VideoHolder(NpgsqlDataReader reader)
        {
            if (ColumnExists(reader, "VideoID"))
            {
                VideoHolderId = Convert.ToInt32(reader["VideoID"].ToString().Trim());
            }

            if (ColumnExists(reader, "YoutubeID"))
            {
                YoutubeId = reader["YoutubeID"].ToString().Trim();
            }

            if (ColumnExists(reader, "ChannelId"))
            {
                ChannelId = Convert.ToInt32(reader["ChannelId"].ToString().Trim());
            }

            if (ColumnExists(reader, "Title"))
            {
                Title = reader["Title"].ToString().Trim();
            }

            if (ColumnExists(reader, "Thumbnail"))
            {
                Thumbnail = reader["Thumbnail"].ToString().Trim();
            }

            if (ColumnExists(reader, "Duration"))
            {
                Duration = reader["Duration"].ToString().Trim();
            }

            if (ColumnExists(reader, "ViewCount"))
            {
                ViewCount = Convert.ToUInt64(reader["ViewCount"].ToString().Trim());
            }

            if (ColumnExists(reader, "PublishedAt"))
            {
                PublishedAt = DateTime.Parse(reader["PublishedAt"].ToString().Trim());
            }
            
        }
        
    }
}
