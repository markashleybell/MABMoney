﻿using MABMoney.Caching;
using MABMoney.Data.Concrete;
using StackExchange.Profiling;
using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

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

            var data = Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue)).Split('-');

            var userId = Convert.ToInt32(data[0]);
            var userEmail = data[1];

            var enableProfiling = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableProfiling"]);

            IDisposable _step = null;

            if (enableProfiling)
            {
                var _profiler = MiniProfiler.Current;
                _step = _profiler.Step("Get Session In AuthenticateAttribute");
            }

            var sessionRepository = new SessionRepository(ConfigurationManager.AppSettings["DataDbConnectionString"], userId);

            var session = sessionRepository.GetByKey(cookieValue);

            if (enableProfiling)
            {
                _step.Dispose();
            }

            if (session == null)
                return false;

            // If caching is enabled, this is the only place we can reliably set up the dependencies
            // TODO: Could these be added on first Cache.Add() if not already present?
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
            context.Items.Add("Email", userEmail);

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var urlHelper = new UrlHelper(filterContext.RequestContext);
            var url = urlHelper.Action("Login", "Users");

            // auth failed, redirect to login page
            filterContext.Result = new RedirectResult(url);
        }
    }
}