using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCollections.Database
{
    public class DBUtil
    {
        public static string Sanitize(object str)
        {
            return str.ToString().Replace("'", "''").Trim();
        }
    }
}
