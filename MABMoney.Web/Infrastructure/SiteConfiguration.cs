using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Infrastructure
{
    public class SiteConfiguration : ISiteConfiguration
    {
        public SiteConfiguration(string sharedSecret, string cookieKey)
        {
            SharedSecret = sharedSecret;
            CookieKey = cookieKey;
        }

        public string SharedSecret { get; private set; }
        public string CookieKey { get; private set; }
    }
}