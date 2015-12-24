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
        public delegate void FollowUp(string id, FollowUp followUp = null);

        public static void PrintChannelUploads(string channelId, FollowUp followUp = null)
        {
            int vidCount = 0;
            ChannelListResponse channel = YoutubeApiHandler.FetchUploadsPlaylistByChannel(channelId, "snippet,contentDetails");

            Console.WriteLine("************* Channel Name: " + channel.Items[0].Snippet.Title + "*************");

            string nextPageToken = string.Empty;
            string uploadsPlaylistId = channel.Items[0].ContentDetails.RelatedPlaylists.Uploads;
            PlaylistItemListResponse searchListResponse;

            do
            {
                searchListResponse = YoutubeApiHandler.FetchPlaylistVideosById(uploadsPlaylistId, nextPageToken, "snippet");
                vidCount += searchListResponse.Items.Count;
                nextPageToken = searchListResponse.NextPageToken;

                if (searchListResponse != null)
                {
                    foreach (var searchResult in searchListResponse.Items)
                    {
                        if (followUp != null)
                        {
                            followUp(searchResult.Snippet.Title);
                        }
                        else
                        {
                            Console.WriteLine(searchResult.Snippet.Title);
                        }
                        
                    }
                }
            }
            while (nextPageToken != null);

            Console.WriteLine("Total Video Count: " + vidCount);

        }

        public static void PrintChannelSubscriptions(string channelId, FollowUp followUp = null)
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
                        
                        if (followUp != null)
                        {
                            followUp(searchResult.Snippet.ResourceId.ChannelId, PrintInfo);
                        }
                        else
                        {
                            Console.WriteLine(searchResult.Snippet.Title);
                        }
                    }
                }
            }
            while (nextPageToken != null);

            Console.WriteLine("Total Subscription Count: " + subscriptionCount);
        }

        private static void PrintInfo(string strToPrint, FollowUp followUp)
        {
            Console.WriteLine(strToPrint);
        }
    }
}
