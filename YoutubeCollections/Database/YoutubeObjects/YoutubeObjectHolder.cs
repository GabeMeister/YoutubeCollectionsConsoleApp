using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCollections.Database.YoutubeObjects
{
    public abstract class YoutubeObjectHolder
    {
        public abstract string FetchInsertSql();
    }
}
