using System;

namespace MABMoney.Web.Infrastructure
{
    public interface IHttpContextProvider
    {
        T GetCookieValue<T>(string key);
        void SetCookie(string key, string value);
        void SetCookie(string key, string value, DateTime expires);
        void SetCookie(string key, string value, DateTime expires, bool httpOnly);
    }
}
