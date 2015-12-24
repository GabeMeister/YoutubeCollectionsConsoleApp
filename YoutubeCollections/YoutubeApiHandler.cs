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
    public class YoutubeApiHandler
    {
        private static string API_KEY = "AIzaSyD3PMEr28Ic6lIRisjCQZ1JO98aqHWLpR4";

        public static PlaylistItemListResponse FetchPlaylistVideosById(string playlistId, string pageToken, string part)
        {
            var ytService = FetchYoutubeService();

            PlaylistItemsResource.ListRequest playlistRequest = ytService.PlaylistItems.List(part);
            playlistRequest.MaxResults = 50;
            playlistRequest.PlaylistId = playlistId;
            playlistRequest.PageToken = pageToken;

            return playlistRequest.Execute();
        }

        public static ChannelListResponse FetchUploadsPlaylistByChannel(string channelId, string part)
        {
            var ytService = FetchYoutubeService();

            ChannelsResource.ListRequest channelRequest = ytService.Channels.List(part);
            channelRequest.MaxResults = 50;
            channelRequest.Id = channelId;

            ChannelListResponse channelResponse = channelRequest.Execute();

            return channelResponse;
        }

        public static SubscriptionListResponse FetchSubscriptionsByChannel(string channelId, string pageToken, string part)
        {
            var ytService = FetchYoutubeService();

            SubscriptionsResource.ListRequest subscriptionRequest = ytService.Subscriptions.List(part);
            subscriptionRequest.MaxResults = 50;
            subscriptionRequest.PageToken = pageToken;
            subscriptionRequest.ChannelId = channelId;

            SubscriptionListResponse subscriptionResponse = subscriptionRequest.Execute();

            return subscriptionResponse;

        }




        private static YouTubeService FetchYoutubeService()
        {
            return new YouTubeService(new BaseClientService.Initializer() { ApiKey = API_KEY });
        }
    }
}
