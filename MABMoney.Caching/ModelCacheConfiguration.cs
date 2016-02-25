using System;
using System.Configuration;

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
