using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace MABMoney.Caching
{
    public class ModelCache : IModelCache
    {
        static MemoryCache _cache = MemoryCache.Default;
        static Dictionary<string, CacheItemInfo> _cacheInfo = new Dictionary<string, CacheItemInfo>();

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

        void IModelCache.Add(string key, object value, int expirationSeconds)
        {
            _cache.Add(key,
                       value,
                       new CacheItemPolicy
                       {
                           AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expirationSeconds)
                       });

            if (!_cacheInfo.ContainsKey(key))
            {
                _cacheInfo[key] = new CacheItemInfo
                {
                    Key = key,
                    Type = value.GetType().ToString(),
                    Hits = 0,
                    Misses = 0
                };
            }

            _cacheInfo[key].Misses++;
        }

        T IModelCache.Get<T>(string key)
        {
            try
            {
                _cacheInfo[key].Hits++;
                return (T)_cache[key];
            }
            catch
            {
                return default(T);
            }
        }

        void IModelCache.Remove(string key)
        {
            _cache.Remove(key);
            _cacheInfo.Remove(key);
        }

        void IModelCache.Clear()
        {
            List<KeyValuePair<String, Object>> cacheItems = (from n in _cache.AsParallel() select n).ToList();

            foreach (KeyValuePair<String, Object> a in cacheItems)
                _cache.Remove(a.Key);

            _cacheInfo.Clear();
        }
    }
}
