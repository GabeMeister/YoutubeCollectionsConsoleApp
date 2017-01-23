using System;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YoutubeCollections.Api
{
    public class YoutubeApiHandler
    {
        private static string API_KEY = "AIzaSyD3PMEr28Ic6lIRisjCQZ1JO98aqHWLpR4";
        private const int MAX_RESULTS = 50;

        public static ChannelListResponse FetchUploadsPlaylistByChannel(string youtubeId, string part)
        {
            var ytService = FetchYoutubeService();

            ChannelsResource.ListRequest channelRequest = ytService.Channels.List(part);
            channelRequest.MaxResults = MAX_RESULTS;
            channelRequest.Id = youtubeId;

            ChannelListResponse channelResponse = channelRequest.Execute();

            return channelResponse;
        }

        public static SubscriptionListResponse FetchSubscriptionsByChannel(string youtubeId, string pageToken, string part)
        {
            var ytService = FetchYoutubeService();

            SubscriptionsResource.ListRequest subscriptionRequest = ytService.Subscriptions.List(part);
            subscriptionRequest.MaxResults = MAX_RESULTS;
            subscriptionRequest.PageToken = pageToken;
            subscriptionRequest.ChannelId = youtubeId;

            SubscriptionListResponse subscriptionResponse = subscriptionRequest.Execute();

            return subscriptionResponse;

        }

        public static PlaylistItemListResponse FetchVideosByPlaylist(string youtubeId, string pageToken, string part)
        {
            var ytService = FetchYoutubeService();

            PlaylistItemsResource.ListRequest playlistRequest = ytService.PlaylistItems.List(part);
            playlistRequest.MaxResults = MAX_RESULTS;

            playlistRequest.PlaylistId = youtubeId;
            playlistRequest.PageToken = pageToken;

            //Console.Write("Requesting playlist ({0},{1})... ", youtubeId, pageToken);
            var request = playlistRequest.Execute();
            //Console.WriteLine("Got playlist! ({0},{1})...", youtubeId, pageToken);

            return request;
        }

        public static PlaylistItemListResponse FetchVideosByPlaylist(string youtubeId, string pageToken, string part, int numResults)
        {
            bool isSuccess = true;
            PlaylistItemListResponse playlistResponse = null;

            do
            {
                try
                {
                    var ytService = FetchYoutubeService();

                    PlaylistItemsResource.ListRequest playlistRequest = ytService.PlaylistItems.List(part);
                    playlistRequest.MaxResults = numResults;
                    playlistRequest.PlaylistId = youtubeId;
                    playlistRequest.PageToken = pageToken;

                    playlistResponse = playlistRequest.Execute();
                    isSuccess = true;
                }
                catch (Exception e)
                {
                    isSuccess = false;
                }
            }
            while (isSuccess != true);
            
            return playlistResponse;
        }

        public static VideoListResponse FetchVideosByIds(string youtubeIds, string part)
        {
            var ytService = FetchYoutubeService();

            VideosResource.ListRequest videoRequest = ytService.Videos.List(part);
            videoRequest.MaxResults = MAX_RESULTS;
            videoRequest.Id = youtubeIds;

            return videoRequest.Execute();
        }
        
        public static bool DoesChannelHavePublicSubscriptions(string youtubeId)
        {
            bool isAllowed = true;

            try
            {
                FetchSubscriptionsByChannel(youtubeId, "", "snippet");
            }
            catch (Exception)
            {
                isAllowed = false;
            }

            return isAllowed;
        }
        

        private static YouTubeService FetchYoutubeService()
        {
            var service = new YouTubeService(new BaseClientService.Initializer() { ApiKey = API_KEY });
            return service;
        }
    }
}
