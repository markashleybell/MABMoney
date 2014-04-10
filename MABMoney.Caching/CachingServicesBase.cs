using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Caching
{
    public abstract class CachingServicesBase
    {
        protected IModelCache _cache;
        protected IModelCacheConfiguration _cacheConfig;

        public CachingServicesBase(IModelCache cache, IModelCacheConfiguration cacheConfig)
        {
            _cache = cache;
            _cacheConfig = cacheConfig;
        }
    }
}
