using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Infrastructure
{
    public class SiteConfiguration : ISiteConfiguration
    {
        public SiteConfiguration(string sharedSecret, 
                                 string cookieKey, 
                                 string noReplyEmailAddress,
                                 string noReplyEmailDisplayName,
                                 string siteUrl)
        {
            SharedSecret = sharedSecret;
            CookieKey = cookieKey;
            NoReplyEmailAddress = noReplyEmailAddress;
            NoReplyEmailDisplayName = noReplyEmailDisplayName;
            SiteUrl = siteUrl;
        }

        public string SharedSecret { get; private set; }
        public string CookieKey { get; private set; }
        public string NoReplyEmailAddress { get; private set; }
        public string NoReplyEmailDisplayName { get; private set; }
        public string SiteUrl { get; private set; }
    }
}