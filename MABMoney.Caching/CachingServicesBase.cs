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

        private void RefreshDependentItems(params string[] dependencies)
        {
            foreach(var dependency in dependencies)
                _cache.Set(_cacheConfig.Get<string>("CookieKey") + "-dependency-" + dependency, Guid.NewGuid(), (int)CacheExpiry.OneHour);
        }

        protected T CacheAndGetValue<T>(string cacheKey, CacheExpiry expiry, Func<T> method)
        {
            return CacheAndGetValue<T>(cacheKey, expiry, method, null);
        }

        protected T CacheAndGetValue<T>(string cacheKey, CacheExpiry expiry, Func<T> method, params string[] dependencies)
        {
            // Append the cookie key to the beginning of the cache key and all dependency keys
            var key = _cacheConfig.Get<string>("CookieKey") + "-" + cacheKey;
            dependencies = dependencies.Select(x => _cacheConfig.Get<string>("CookieKey") + "-dependency-" + x).ToArray();

            var item = _cache.Get<T>(key);

            if (item == null)
            {
                item = (T)method();
                _cache.Add(key, item, (int)expiry, dependencies);
            }

            return item;
        }
    }
}
