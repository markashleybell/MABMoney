using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Infrastructure
{
    public interface ICacheProvider
    {
        bool Add(string key, object value);
        bool Replace(string key, object value);
        bool Set(string key, object value);
        T Get<T>(string key);
    }
}