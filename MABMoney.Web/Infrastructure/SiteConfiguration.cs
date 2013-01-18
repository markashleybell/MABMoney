using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Infrastructure
{
    public class SiteConfiguration : ISiteConfiguration
    {
        public SiteConfiguration(string sharedSecret)
        {
            SharedSecret = sharedSecret;
        }

        public string SharedSecret { get; private set; }
    }
}