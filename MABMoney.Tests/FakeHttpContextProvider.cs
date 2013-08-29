using MABMoney.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Tests
{
    public class FakeHttpContextProvider : IHttpContextProvider
    {
        public Dictionary<string, Tuple<string, DateTime>> Cookies = new Dictionary<string, Tuple<string, DateTime>>();

        public void SetCookie(string key, string value)
        {
            var values = Tuple.Create<string, DateTime>(value, DateTime.MinValue);

            if (!Cookies.ContainsKey(key))
                Cookies.Add(key, values);
            else
                Cookies[key] = values;
        }

        public void SetCookie(string key, string value, DateTime expires)
        {
            var values = Tuple.Create<string, DateTime>(value, expires);

            if (!Cookies.ContainsKey(key))
                Cookies.Add(key, values);
            else
                Cookies[key] = values;
        }
    }
}
