using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCollections.ObjectHolders
{
    public class CollectionHolder : ApiResponseHolder
    {
        public ulong? CollectionHolderId { get; set; }
        public ulong? OwnerChannelId { get; set; }
        public string OwnerYoutubeChannelId { get; set; }
        public string Title { get; set; }
    }
}
