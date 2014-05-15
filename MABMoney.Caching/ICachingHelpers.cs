using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Caching
{
    public interface ICachingHelpers
    {
        string GetCacheKey(string key);
        string GetDependencyKey(string key);
        string GetDependencyKey(string key, string userId);
    }
}
