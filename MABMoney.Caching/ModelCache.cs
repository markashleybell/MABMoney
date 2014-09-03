using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace MABMoney.Caching
{
    public class ModelCache : IModelCache
    {
        IModelCacheConfiguration _config;
        static MemoryCache _cache = MemoryCache.Default;
        static Dictionary<string, CacheItemInfo> _cacheInfo = new Dictionary<string, CacheItemInfo>();

        public ModelCache(IModelCacheConfiguration config)
        {
            _config = config;
        }

        public List<CacheItemInfo> Items
        {
            get
            {
                List<CacheItemInfo> output = new List<CacheItemInfo>();
                foreach (var item in _cache)
                {
                    if (_cacheInfo.ContainsKey(item.Key))
                    {
                        output.Add(_cacheInfo[item.Key]);
                    }
                    else
                    {
                        output.Add(new CacheItemInfo() { Key = "Not Found: " + item.Key, Hits = -1, Misses = 0, Type = "" });
                    }
                }

                return output;
            }
        }

        public object this[string key]
        {
            get { return _cache[key]; }
        }

        public void Add(string key, object value, int expirationSeconds)
        {
            Add(key, value, expirationSeconds, null);
        }

        public void Add(string key, object value, int expirationSeconds, params string[] dependencies)
        {
            var policy = new CacheItemPolicy {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expirationSeconds)
            };

            if (dependencies != null && dependencies.Count() > 0)
            {
                policy.ChangeMonitors.Add(
                    _cache.CreateCacheEntryChangeMonitor(dependencies)
                );
            }

            _cache.Add(key, new CacheItem { Value = value }, policy);

            if (!_cacheInfo.ContainsKey(key))
            {
                _cacheInfo[key] = new CacheItemInfo
                {
                    Key = key,
                    Type = (value == null) ? "null" : value.GetType().ToString(),
                    Value = (value == null) ? "null" : value.ToString(),
                    Hits = 0,
                    Misses = 0
                };
            }

            _cacheInfo[key].Misses++;
        }

        public T Get<T>(string key)
        {
            try
            {
                _cacheInfo[key].Hits++;
                return (T)((CacheItem)_cache[key]).Value;
            }
            catch
            {
                return default(T);
            }
        }

        public void Set(string key, object value, int expirationSeconds)
        {
            _cache.Set(key, value, DateTimeOffset.Now.AddSeconds(expirationSeconds));
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _cacheInfo.Remove(key);
        }

        public void Clear()
        {
            List<KeyValuePair<String, Object>> cacheItems = (from n in _cache.AsParallel() select n).ToList();

            foreach (KeyValuePair<String, Object> a in cacheItems)
                _cache.Remove(a.Key);

            _cacheInfo.Clear();
        }

        public void InvalidateAllWithDependency(string dependencyKey)
        {
            _cache.Set(dependencyKey, 
                       Guid.NewGuid(), 
                       DateTimeOffset.Now.AddSeconds((int)CacheExpiry.OneHour));
        }

        public bool ContainsKey(string key)
        {
            return _cache[key] != null;
        }
    }
}
