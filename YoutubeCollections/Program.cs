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
        static string xKito = "UCMOgdURr7d8pOVlc-alkfRg";
        static string theVibeGuide = "UCxH0sQJKG6Aq9-vFIPnDZ2A";
        static string NoCopyrightSounds = "UC_aEa8K-EOJ3D6gOs7HcyNg";
        static string KoanSound = "UCazuZ5iU8LfQ6-IucPhctKQ";
        static string BrothersGreenEats = "UCzH5n3Ih5kgQoiDAQt2FwLw";

        static string ChannelsListFile = @"C:\Users\Gabe\Desktop\Channels.py";

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("YouTube Data API");
            Console.WriteLine("============================");
            

            try
            {
                //YoutubeWrapper.FetchChannelSubscriptions(myChannel);
                //YoutubeWrapper.FetchChannelUploads(BrothersGreenEats);
                //YoutubeWrapper.FetchChannelUploadsFromStream(new StreamReader(ChannelsListFile));
                //YoutubeWrapper.AddPublishedAtTimeStamps();
                YoutubeWrapper.BuildThumbnailCollage();
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