using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MABMoney.Web.Infrastructure
{
    public interface ISiteConfiguration
    {
        string SharedSecret { get; }
        string CookieKey { get; }
        string NoReplyEmailAddress { get; }
        string NoReplyEmailDisplayName { get; }
        string SiteUrl { get; }
        decimal DefaultCardPaymentAmount { get; }
    }
}