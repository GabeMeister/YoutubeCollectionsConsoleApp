using System;


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
        static string AssociatedPress = "UC52X5wxOL_s5yw0dQk7NtgA";
        static string WildFilmsIndia = "UCixvwLpO_pk4uVOkkkqP3Mw";
        static string TheYoungTurks = "UC1yBKRuGpC1tSM73A0ZjYjQ";
        static string TVCultura = "UCjOJvvYe6tyEHY21OD33h8A";
        static string TheTelegraph = "UCPgLNge0xqQHWM5B5EFH9Cg";
        static string TomoNewsUS = "UCt-WqkTyKK1_70U4bb4k4lQ";

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


                //YoutubeTasks.FetchMissingChannelUploads(myChannel);
                //YoutubeTasks.UpdateAllMissingChannelUploads();
                //YoutubeTasks.FetchAllSubscriptionsToAllChannels();
                //YoutubeTasks.InsertCollectionsData();
                //YoutubeTasks.DeleteCollectionsData();
                //YoutubeTasks.UpdateAllVideoInfo();
                YoutubeTasks.ThreadedFetchExistingChannelUploads();

                
                


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