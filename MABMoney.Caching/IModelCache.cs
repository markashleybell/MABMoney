using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Caching
{
    public interface IModelCache
    {
        List<CacheItemInfo> BaseCache { get; }
        object this[string key] { get; }
        void Add(string key, object value, int expirationSeconds);
        T Get<T>(string key);
        void Remove(string key);
        void Clear();
    }
}
