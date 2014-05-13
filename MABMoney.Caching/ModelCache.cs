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
        private int _userId;

        public ModelCache(IModelCacheConfiguration config, int userId)
        {
            _config = config;
            _userId = userId;
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
            get { return _cache[GetUserKey(key)]; }
        }

        public void Add(string key, object value, int expirationSeconds)
        {
            Add(key, value, expirationSeconds, null);
        }

        public void Add(string key, object value, int expirationSeconds, params string[] dependencies)
        {
            if (value == null)
                return;

            var policy = new CacheItemPolicy {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expirationSeconds)
            };

            if (dependencies != null && dependencies.Count() > 0)
            {
                policy.ChangeMonitors.Add(
                    _cache.CreateCacheEntryChangeMonitor(dependencies.Select(d => GetUserKey(d)))
                );
            }

            _cache.Add(GetUserKey(key), value, policy);

            if (!_cacheInfo.ContainsKey(GetUserKey(key)))
            {
                _cacheInfo[GetUserKey(key)] = new CacheItemInfo
                {
                    Key = GetUserKey(key),
                    Type = value.GetType().ToString(),
                    Value = value.ToString(),
                    Hits = 0,
                    Misses = 0
                };
            }

            _cacheInfo[GetUserKey(key)].Misses++;
        }

        public T Get<T>(string key)
        {
            try
            {
                _cacheInfo[GetUserKey(key)].Hits++;
                return (T)_cache[GetUserKey(key)];
            }
            catch
            {
                return default(T);
            }
        }

        public void Set(string key, object value, int expirationSeconds)
        {
            _cache.Set(GetUserKey(key), value, DateTimeOffset.Now.AddSeconds(expirationSeconds));
        }

        public void Remove(string key)
        {
            _cache.Remove(GetUserKey(key));
            _cacheInfo.Remove(GetUserKey(key));
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
            _cache.Set(_config.Get<string>("CookieKey") + "-dependency-" + GetUserKey(dependencyKey), 
                       Guid.NewGuid(), 
                       DateTimeOffset.Now.AddSeconds((int)CacheExpiry.OneHour));
        }

        private string GetUserKey(string key)
        {
            return (_userId == -1) ? key : key + "-" + _userId.ToString();
        }
    }
}
