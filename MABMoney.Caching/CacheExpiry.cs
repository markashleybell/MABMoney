using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MABMoney.Caching
{
    public enum CacheExpiry : int
    {
        FiveMinutes = 300,
        TenMinutes = 600,
        FifteenMinutes = 900,
        TwentyMinutes = 1200,
        ThirtyMinutes = 1800,
        OneHour = 3600,
        ThreeHours = 10800,
        TwelveHours = 43200
    }
}
