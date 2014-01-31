using MABMoney.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Infrastructure
{
    public class HttpContextProvider : IHttpContextProvider
    {
        private HttpContextBase _context;

        public HttpContextProvider(HttpContextBase context)
        {
            _context = context;
        }

        public T GetCookieValue<T>(string key)
        {
            var cookie = _context.Request.Cookies[key];
            return (cookie != null) ? (T)Convert.ChangeType(cookie.Value, typeof(T)) : default(T);
        }

        public void SetCookie(string key, string value)
        {
            _context.Response.Cookies.Add(new HttpCookie(key, value));
        }

        public void SetCookie(string key, string value, DateTime expires)
        {
            var cookie = new HttpCookie(key, value);
            cookie.Expires = expires;

            _context.Response.Cookies.Add(cookie);
        }
    }
}