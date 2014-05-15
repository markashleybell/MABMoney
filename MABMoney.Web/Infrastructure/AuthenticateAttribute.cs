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

            var encryptedCookieValue = context.Request.Cookies[ConfigurationManager.AppSettings["CookieKey"]].Value;

            IDisposable _step;
            var _profiler = MiniProfiler.Current;

            _step = _profiler.Step("Decrypt Auth Cookie");
            var userId = Convert.ToInt32(EncryptionHelpers.DecryptStringAES(encryptedCookieValue, ConfigurationManager.AppSettings["SharedSecret"]));
            _step.Dispose();

            _step = _profiler.Step("Instantiate DB Factory and Service");
            
            DataStoreFactory dataStoreFactory;

            if (ConfigurationManager.AppSettings["ExternalDbConnectionString"] != null)
                dataStoreFactory = new DataStoreFactory(userId, new DateTimeProvider(() => DateTime.Now), ConfigurationManager.AppSettings["ExternalDbConnectionString"]);
            else
                dataStoreFactory = new DataStoreFactory(userId, new DateTimeProvider(() => DateTime.Now));

            var unitOfWork = new UnitOfWork(dataStoreFactory);
            IUserServices userServices;

            if (ConfigurationManager.AppSettings["EnableCaching"] != null)
            {
                var cacheConfiguration = new ModelCacheConfiguration();
                var nonCachingUserServices = new UserServices(new Repository<User, int>(unitOfWork), unitOfWork, new DateTimeProvider(() => DateTime.Now));
                userServices = new CachingUserServices(nonCachingUserServices, new ModelCache(cacheConfiguration), new CachingHelpers(cacheConfiguration, userId));
            }
            else
            {
                userServices = new UserServices(new Repository<User, int>(unitOfWork), unitOfWork, new DateTimeProvider(() => DateTime.Now));
            }
            
            _step.Dispose();

            _step = _profiler.Step("Get Minimal User Data");
            var user = userServices.GetMinimal(userId);
            _step.Dispose();

            if (user == null)
                return false;

            context.Items.Add("UserID", user.UserID);
            context.Items.Add("IsAdmin", user.IsAdmin);
            context.Items.Add("UserEmail", user.Email);

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