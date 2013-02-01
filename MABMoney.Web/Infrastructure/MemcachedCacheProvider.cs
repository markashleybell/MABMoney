using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Infrastructure
{
    public class MemcachedCacheProvider : ICacheProvider
    {
        private MemcachedClient _client;

        public MemcachedCacheProvider(MemcachedClientConfiguration configuration)
        {
            _client = new MemcachedClient(configuration);
        }

        public bool Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        public bool Replace(string key, object value)
        {
            throw new NotImplementedException();
        }

        public bool Set(string key, object value)
        {
            return _client.Store(StoreMode.Set, key, value);
        }

        public T Get<T>(string key)
        {
            return _client.Get<T>(key);
        }
    }
}