using System;
using System.Linq;

namespace MABMoney.Caching
{
    public abstract class CachingServicesBase
    {
        protected IModelCache _cache;
        protected ICachingHelpers _helpers;

        public CachingServicesBase(IModelCache cache, ICachingHelpers helpers)
        {
            _cache = cache;
            _helpers = helpers;
        }

        protected T CacheAndGetValue<T>(string cacheKey, CacheExpiry expiry, Func<T> method)
        {
            return CacheAndGetValue<T>(cacheKey, expiry, method, null);
        }

        protected T CacheAndGetValue<T>(string cacheKey, CacheExpiry expiry, Func<T> method, params string[] dependencies)
        {
            // Append the cookie key to the beginning of the cache key and all dependency keys
            var key = _helpers.GetCacheKey(cacheKey);
            dependencies = dependencies.Select(x => _helpers.GetDependencyKey(x)).ToArray();

            if (_cache.ContainsKey(key))
            {
                return _cache.Get<T>(key);
            }
            else
            {
                var item = (T)method();
                _cache.Add(key, item, (int)expiry, dependencies);
                return item;
            }
        }
    }
}
