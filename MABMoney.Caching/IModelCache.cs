using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Caching
{
    public interface IModelCache
    {
        List<CacheItemInfo> Items { get; }
        object this[string key] { get; }
        void Add(string key, object value, int expirationSeconds);
        void Add(string key, object value, int expirationSeconds, params string[] dependencies);
        T Get<T>(string key);
        void Set(string key, object value, int expirationSeconds);
        void Remove(string key);
        void Clear();
    }
}
