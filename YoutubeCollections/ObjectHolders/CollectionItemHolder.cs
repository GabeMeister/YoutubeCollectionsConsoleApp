using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCollections.ObjectHolders
{
    public class CollectionItemHolder : ObjectHolder
    {
        public ulong? CollectionItemHolderId { get; set; }
        public ulong? CollectionId { get; set; }
        public ulong? ItemChannelId { get; set; }
    }
}
