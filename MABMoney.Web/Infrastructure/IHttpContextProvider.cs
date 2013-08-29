using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Web.Infrastructure
{
    public interface IHttpContextProvider
    {
        void SetCookie(string key, string value);
        void SetCookie(string key, string value, DateTime expires);
    }
}
