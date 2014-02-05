using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Infrastructure
{
    public class SiteConfiguration : ISiteConfiguration
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