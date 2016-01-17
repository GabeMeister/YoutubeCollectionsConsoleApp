/*
*/
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Text;

namespace YoutubeCollections
{
    /// <summary>
    /// YouTube Data API v3 sample: retrieve my uploads.
    /// Relies on the Google APIs Client Library for .NET, v1.7.0 or higher.
    /// See https://code.google.com/p/google-api-dotnet-client/wiki/GettingStarted
    /// </summary>
    internal class YoutubeCollectionsApp
    {
        static string myChannel = "UC4LVLoBN0xbOb5xJuA0ia9A";
        static string myUploadsPlaylist = "UU4LVLoBN0xbOb5xJuA0ia9A";

        static string xKito = "UCMOgdURr7d8pOVlc-alkfRg";
        static string theVibeGuide = "UCxH0sQJKG6Aq9-vFIPnDZ2A";
        static string NoCopyrightSounds = "UC_aEa8K-EOJ3D6gOs7HcyNg";
        static string KoanSound = "UCazuZ5iU8LfQ6-IucPhctKQ";
        static string BrothersGreenEats = "UCzH5n3Ih5kgQoiDAQt2FwLw";
        static string PewDiePie = "UC-lHJZR3Gqxm24_Vd_AJ5Yw";
        static string GEazyMusicVEVO = "UCjjC1Jk_1o1VWpLalgo21XQ";
        static string JimmyFallon = "UC8-Th83bH_thdKZDJCrn88g";
        static string BayHey264 = "UCpAhhtiVgTWFEDASAcfV-iQ";

        static string ChannelsListFile = @"C:\Users\Gabe\Desktop\Channels.py";

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("YouTube Data API");
            Console.WriteLine("============================");


            try
            {
                //YoutubeTasks.FetchAllUploadsForAllChannelSubscriptions(PewDiePie);
                //YoutubeTasks.FetchChannelUploads(GEazyMusicVEVO);
                //YoutubeTasks.RecordChannelSubscriptions(BrothersGreenEats);
                //YoutubeTasks.DetectChannelSubscriptions();


                //YoutubeTasks.UpdateAllMissingChannelUploads();
                //YoutubeTasks.FetchMissingChannelUploads(myChannel);
                YoutubeTasks.UpdateAllMissingChannelUploads();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
/*
*/
//using System;
//using System.IO;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;

//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Services;
//using Google.Apis.Upload;
//using Google.Apis.Util.Store;
//using Google.Apis.YouTube.v3;
//using Google.Apis.YouTube.v3.Data;

//namespace Google.Apis.YouTube.Samples
//{
//    /// <summary>
//    /// YouTube Data API v3 sample: retrieve my uploads.
//    /// Relies on the Google APIs Client Library for .NET, v1.7.0 or higher.
//    /// See https://code.google.com/p/google-api-dotnet-client/wiki/GettingStarted
//    /// </summary>
//    internal class MyUploads
//    {
//        [STAThread]
//        static void Main(string[] args)
//        {
//            Console.WriteLine("YouTube Data API: My Uploads");
//            Console.WriteLine("============================");

//            try
//            {
//                new MyUploads().Run().Wait();
//            }
//            catch (AggregateException ex)
//            {
//                foreach (var e in ex.InnerExceptions)
//                {
//                    Console.WriteLine("Error: " + e.Message);
//                }
//            }

//            Console.WriteLine("Press any key to continue...");
//            Console.ReadKey();
//        }

//        private async Task Run()
//        {
//            UserCredential credential;

//            using (var stream = new FileStream(@"E:\Documents\Coding Projects\YoutubeCollections\YoutubeCollections\client_secrets.json", FileMode.Open, FileAccess.Read))
//            {
//                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
//                    GoogleClientSecrets.Load(stream).Secrets,
//                    // This OAuth 2.0 access scope allows for read-only access to the authenticated 
//                    // user's account, but not other types of account access.
//                    new[] { YouTubeService.Scope.YoutubeReadonly },
//                    "user",
//                    CancellationToken.None,
//                    new FileDataStore(this.GetType().ToString())
//                );
//                Console.WriteLine(credential.UserId);
//            }

//            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
//            {
//                HttpClientInitializer = credential,
//                ApplicationName = this.GetType().ToString()
//            });

//            var channelsListRequest = youtubeService.Channels.List("contentDetails");
//            channelsListRequest.Mine = true;

//            // Retrieve the contentDetails part of the channel resource for the authenticated user's channel.
//            var channelsListResponse = await channelsListRequest.ExecuteAsync();

//            foreach (var channel in channelsListResponse.Items)
//            {
//                // From the API response, extract the playlist ID that identifies the list
//                // of videos uploaded to the authenticated user's channel.
//                var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;

//                Console.WriteLine("Videos in list {0}", uploadsListId);

//                var nextPageToken = "";
//                while (nextPageToken != null)
//                {
//                    var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
//                    playlistItemsListRequest.PlaylistId = uploadsListId;
//                    playlistItemsListRequest.MaxResults = 50;
//                    playlistItemsListRequest.PageToken = nextPageToken;

//                    // Retrieve the list of videos uploaded to the authenticated user's channel.
//                    var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();

//                    foreach (var playlistItem in playlistItemsListResponse.Items)
//                    {
//                        // Print information about each video.
//                        Console.WriteLine("{0} ({1})", playlistItem.Snippet.Title, playlistItem.Snippet.ResourceId.VideoId);
//                    }

//                    nextPageToken = playlistItemsListResponse.NextPageToken;
//                }
//            }
//        }
//    }
//}