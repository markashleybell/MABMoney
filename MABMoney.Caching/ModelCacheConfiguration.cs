using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MABMoney.Caching
{
    public class ModelCacheConfiguration : IModelCacheConfiguration
    {
        public string this[string key]
        {
            get { return ConfigurationManager.AppSettings[key]; }
        }

        public T Get<T>(string key)
        {
            return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
        }
    }
}
