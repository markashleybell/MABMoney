using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MABMoney.Web.Infrastructure
{
    public interface ISiteConfiguration
    {
        string this[string key] { get; }
        T Get<T>(string key);
    }
}