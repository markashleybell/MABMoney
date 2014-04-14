using mab.lib.SimpleMapper;
using MABMoney.Caching;
using MABMoney.Data;
using MABMoney.Web.Infrastructure;
using MABMoney.Web.Models;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;
using StackExchange.Profiling.Storage;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MABMoney.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var enableProfiling = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableProfiling"]);
            var enableMigrations = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableMigrations"]);
            var enableCaching = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableCaching"]);

            var sharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
            var cookieKey = ConfigurationManager.AppSettings["CookieKey"];
            var externalDbConnectionString = ConfigurationManager.AppSettings["ExternalDbConnectionString"];
            var profilerConnectionString = ConfigurationManager.AppSettings["ProfilerDbConnectionString"];

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(ProfileViewModel), new ProfileModelBinder());

            if (enableProfiling)
            {
                if (profilerConnectionString != null)
                    MiniProfiler.Settings.Storage = new SqlServerStorage(profilerConnectionString);

                MiniProfilerEF6.Initialize();
            }

            // Remove the XML formatter so that Web API actions return JSON by default
            var config = GlobalConfiguration.Configuration;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            //  If migrations are enabled, set up the initialiser based on the connection string
            if (enableMigrations)
            {
                if (externalDbConnectionString != null)
                    Database.SetInitializer<DataStore>(new MigrateDatabaseToLatestVersionWithConnectionString<DataStore, MABMoney.Data.Migrations.Configuration>(externalDbConnectionString));
                else
                    Database.SetInitializer<DataStore>(new MigrateDatabaseToLatestVersion<DataStore, MABMoney.Data.Migrations.Configuration>());
            }
            else
            {
                Database.SetInitializer<DataStore>(null);
            }

            ICryptoProvider _crypto = new CryptoWrapper();

            Mapper.AddMapping<MABMoney.Web.Models.Users.SignupViewModel, MABMoney.Services.DTO.UserDTO>((s, d) =>
            {
                d.Forename = s.Forename;
                d.Surname = s.Surname;
                d.Email = s.Email;
                d.Password = _crypto.HashPassword(s.Password);
            });

            Mapper.AddMapping<MABMoney.Web.Models.Users.CreateViewModel, MABMoney.Services.DTO.UserDTO>((s, d) =>
            {
                d.Forename = s.Forename;
                d.Surname = s.Surname;
                d.Email = s.Email;
                d.Password = _crypto.HashPassword(s.Password);
            });

            Mapper.AddMapping<MABMoney.Services.DTO.UserDTO, MABMoney.Web.Models.Users.EditViewModel>((s, d) =>
            {
                d.UserID = s.UserID;
                d.Forename = s.Forename;
                d.Surname = s.Surname;
                d.Email = s.Email;
            });

            Mapper.AddMapping<MABMoney.Web.Models.Users.EditViewModel, MABMoney.Services.DTO.UserDTO>((s, d) =>
            {
                d.UserID = s.UserID;
                d.Forename = s.Forename;
                d.Surname = s.Surname;
                d.Email = s.Email;
                d.Password = (s.Password != null) ? _crypto.HashPassword(s.Password) : null;
            });

            // Add the cache dependency items to the cache
            new ModelCache().Add(cookieKey + "-dependency-all", Guid.NewGuid().ToString(), (int)CacheExpiry.OneHour);
            new ModelCache().Add(cookieKey + "-dependency-user", Guid.NewGuid().ToString(), (int)CacheExpiry.OneHour);
        }

        protected void Application_BeginRequest()
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableProfiling"]))
                MiniProfiler.Start();
        }

        protected void Application_EndRequest()
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableProfiling"]))
                MiniProfiler.Stop();
        }
    }
}