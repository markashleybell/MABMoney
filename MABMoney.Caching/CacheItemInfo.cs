using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MABMoney.Caching
{
    public class CacheItemInfo
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public int Hits { get; set; }
        public int Misses { get; set; }
    }
}
