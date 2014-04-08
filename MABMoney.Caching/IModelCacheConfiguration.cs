using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Caching
{
    public interface IModelCacheConfiguration
    {
        string this[string key] { get; }
        T Get<T>(string key);
    }
}
