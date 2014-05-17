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
using System.Linq;
using System.Linq.Expressions;

namespace MABMoney.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        private bool enableProfiling = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableProfiling"]);
        private bool enableMigrations = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableMigrations"]);
        private bool enableCaching = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableCaching"]);

        private string sharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
        private string cookieKey = ConfigurationManager.AppSettings["CookieKey"];
        private string externalDbConnectionString = ConfigurationManager.AppSettings["ExternalDbConnectionString"];
        private string profilerConnectionString = ConfigurationManager.AppSettings["ProfilerDbConnectionString"];

        private string[] profileExcludes = new string[] { "/home/showcachecontents", "/home/invalidatecache" };

        protected void Application_Start()
        {
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
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (enableProfiling && !profileExcludes.Contains(Request.Url.AbsolutePath.ToLower()))
                MiniProfiler.Start();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (enableProfiling && !profileExcludes.Contains(Request.Url.AbsolutePath.ToLower()))
                MiniProfiler.Stop();
        }
    }
}