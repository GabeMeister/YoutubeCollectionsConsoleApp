using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YoutubeCollections
{
    public class YoutubeWrapper
    {
        private const int MAX_RESULTS = 50;

        public static void FetchChannelSubscriptions(string channelId)
        {
            // NOTE: cannot view other channel subscriptions

            int subscriptionCount = 0;
            string nextPageToken = string.Empty;
            SubscriptionListResponse subscriptionsList;

            do
            {
                subscriptionsList = YoutubeApiHandler.FetchSubscriptionsByChannel(channelId, nextPageToken, "snippet");
                subscriptionCount += subscriptionsList.Items.Count;
                nextPageToken = subscriptionsList.NextPageToken;

                if (subscriptionsList != null)
                {
                    foreach (var searchResult in subscriptionsList.Items)
                    {
                        FetchChannelUploads(searchResult.Snippet.ResourceId.ChannelId);
                    }
                }
            }
            while (nextPageToken != null);

            Console.WriteLine("Total Subscription Count: " + subscriptionCount);
        }

        public static void FetchChannelUploads(string channelId)
        {
            int vidCount = 0;
            ChannelListResponse channel = YoutubeApiHandler.FetchUploadsPlaylistByChannel(channelId, "snippet,contentDetails");

            Console.WriteLine("************* " + channel.Items[0].Snippet.Title + " | " + channel.Items[0].Id + " *************");

            string nextPageToken = string.Empty;
            string uploadsPlaylistId = channel.Items[0].ContentDetails.RelatedPlaylists.Uploads;
            PlaylistItemListResponse searchListResponse;

            do
            {
                searchListResponse = YoutubeApiHandler.FetchVideosByPlaylist(uploadsPlaylistId, nextPageToken, "snippet");
                vidCount += searchListResponse.Items.Count;
                nextPageToken = searchListResponse.NextPageToken;

                if (searchListResponse != null)
                {
                    string videoIds = string.Empty;

                    if (searchListResponse.Items != null && searchListResponse.Items.Count > 0)
                    {
                        foreach (var searchResult in searchListResponse.Items)
                        {
                            videoIds += searchResult.Snippet.ResourceId.VideoId + ",";
                        }

                        // Remove last comma
                        videoIds = videoIds.Substring(0, videoIds.Length - 1);

                        FetchVideoInfo(videoIds);
                    }
                    
                }
            }
            while (nextPageToken != null);

            Console.WriteLine("Total Video Count: " + vidCount);

        }

        public static void FetchVideoInfo(string videoIds)
        {
            VideoListResponse videos = YoutubeApiHandler.FetchVideoById(videoIds, "snippet,contentDetails,statistics");

            foreach(var video in videos.Items)
            {
                Console.WriteLine("====================");
                Console.WriteLine(video.Snippet.Title);
                Console.WriteLine(video.Snippet.ChannelTitle);
                Console.WriteLine(video.Snippet.Thumbnails.Medium.Url);
                Console.WriteLine(video.ContentDetails.Duration);
                Console.WriteLine(video.Statistics.ViewCount);
                
            }
            
        }

        

        private static void PrintInfo(string strToPrint)
        {
            Console.WriteLine(strToPrint);
        }
    }
}
