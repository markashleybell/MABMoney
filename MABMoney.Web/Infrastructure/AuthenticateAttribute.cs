﻿using MABMoney.Services;
using MABMoney.Data;
using MABMoney.Domain;
using MABMoney.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using StackExchange.Profiling;
using MABMoney.Caching;
using System.Text;

namespace MABMoney.Web.Infrastructure
{
    public class AuthenticateAttribute : AuthorizeAttribute
    {
        // This method must be thread-safe since it is called by the thread-safe OnCacheAuthorization() method.
        protected override bool AuthorizeCore(HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.Request.Cookies[ConfigurationManager.AppSettings["CookieKey"]] == null)
                return false;

            var cookieValue = context.Request.Cookies[ConfigurationManager.AppSettings["CookieKey"]].Value;

            IDisposable _step;
            var _profiler = MiniProfiler.Current;

            _step = _profiler.Step("Decrypt Auth Cookie");
            var userId = Convert.ToInt32(Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue)).Split('-')[0]);
            _step.Dispose();

            _step = _profiler.Step("Get Session In AuthenticateAttribute");

            DataStoreFactory dataStoreFactory;

            if (ConfigurationManager.AppSettings["ExternalDbConnectionString"] != null)
                dataStoreFactory = new DataStoreFactory(userId, new DateTimeProvider(() => DateTime.Now), ConfigurationManager.AppSettings["ExternalDbConnectionString"]);
            else
                dataStoreFactory = new DataStoreFactory(userId, new DateTimeProvider(() => DateTime.Now));

            var unitOfWork = new UnitOfWork(dataStoreFactory);

            var sessionServices = new SessionServices(new DateTimeProvider(() => DateTime.Now), new Repository<Session, int>(unitOfWork), unitOfWork);

            var session = sessionServices.GetByUserAndKey(userId, cookieValue);

            _step.Dispose();

            if (session == null)
                return false;

            if (ConfigurationManager.AppSettings["EnableCaching"] != null && Convert.ToBoolean(ConfigurationManager.AppSettings["EnableCaching"]) == true)
            {
                var cacheConfiguration = new ModelCacheConfiguration();
                var cache = new ModelCache(cacheConfiguration);
                var cachingHelpers = new CachingHelpers(cacheConfiguration, userId);

                if (cache.Items.FirstOrDefault(x => x.Key == cachingHelpers.GetDependencyKey("all", userId.ToString())) == null)
                {
                    // Add the cache dependency items to the cache
                    // We have to manually add the user ID to the keys here because it wasn't necessarily present when the cache object was injected
                    cache.Add(cachingHelpers.GetDependencyKey("all", userId.ToString()), Guid.NewGuid().ToString(), (int)CacheExpiry.FifteenMinutes);
                    cache.Add(cachingHelpers.GetDependencyKey("user", userId.ToString()), Guid.NewGuid().ToString(), (int)CacheExpiry.FifteenMinutes);
                    cache.Add(cachingHelpers.GetDependencyKey("transaction", userId.ToString()), Guid.NewGuid().ToString(), (int)CacheExpiry.FifteenMinutes);
                    cache.Add(cachingHelpers.GetDependencyKey("budget", userId.ToString()), Guid.NewGuid().ToString(), (int)CacheExpiry.FifteenMinutes);
                    cache.Add(cachingHelpers.GetDependencyKey("account", userId.ToString()), Guid.NewGuid().ToString(), (int)CacheExpiry.FifteenMinutes);
                    cache.Add(cachingHelpers.GetDependencyKey("category", userId.ToString()), Guid.NewGuid().ToString(), (int)CacheExpiry.FifteenMinutes);
                }
            }

            context.Items.Add("UserID", userId);

            return true;
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (AuthorizeCore(filterContext.HttpContext))
            {
                // ** IMPORTANT **
                // Since we're performing authorization at the action level, the authorization code runs
                // after the output caching module. In the worst case this could allow an authorized user
                // to cause the page to be cached, then an unauthorized user would later be served the
                // cached page. We work around this by telling proxies not to cache the sensitive page,
                // then we hook our custom authorization code into the caching mechanism so that we have
                // the final say on whether a page should be served from the cache.

                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
            }
            else
            {
                // auth failed, redirect to login page
                filterContext.Result = new RedirectResult("/Users/Login");
                //filterContext.Result = new RedirectResult("/Users/Login?returnUrl=" +
                //filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
            }
        }

    }
}