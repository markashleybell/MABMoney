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
        private ISiteConfiguration _config;

        public HttpContextProvider(HttpContextBase context, ISiteConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public T GetCookieValue<T>(string key)
        {
            var cookie = _context.Request.Cookies[key];
            return (cookie != null) ? (T)Convert.ChangeType(cookie.Value, typeof(T)) : default(T);
        }

        public void SetCookie(string key, string value)
        {
            var cookie = new HttpCookie(key, value);
            cookie.HttpOnly = true;
            cookie.Secure = true;
            cookie.Domain = _config.Get<string>("CookieDomain");

            _context.Response.Cookies.Add(cookie);
        }

        public void SetCookie(string key, string value, DateTime expires)
        {
            var cookie = new HttpCookie(key, value);
            cookie.Expires = expires;
            cookie.HttpOnly = true;
            cookie.Secure = true;
            cookie.Domain = _config.Get<string>("CookieDomain");

            _context.Response.Cookies.Add(cookie);
        }

        public void SetCookie(string key, string value, DateTime expires, bool httpOnly)
        {
            var cookie = new HttpCookie(key, value);
            cookie.Expires = expires;
            cookie.HttpOnly = httpOnly;
            cookie.Secure = true;
            cookie.Domain = _config.Get<string>("CookieDomain");

            _context.Response.Cookies.Add(cookie);
        }
    }
}