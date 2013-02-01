using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using MABMoney.Domain;
using System.Data.Entity;
using MABMoney.Data;
using MABMoney.Web.Infrastructure;
using Microsoft.Practices.Unity;
using MABMoney.Services;
using MABMoney.Web.Models;
using System.Configuration;
using StackExchange.Profiling;
using MABMoney.Web.Helpers;
using mab.lib.SimpleMapper;
using MABMoney.Services.DTO;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;



namespace MABMoney.Web
{
    public static class Global
    {
        public static MemcachedClient Cache { get; set; }
    }

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.Add(typeof(ProfileViewModel), new ProfileModelBinder());

            var sharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
            var cookieKey = ConfigurationManager.AppSettings["CookieKey"];

            // Set up object mappings for Unity DI
            var container = new UnityContainer();

            // TODO: Look into object lifetime management
            container.RegisterType<IRepository<User, int>, Repository<User, int>>(new HttpContextLifetimeManager<User>())
                     .RegisterType<IRepository<Account, int>, Repository<Account, int>>(new HttpContextLifetimeManager<Account>())
                     .RegisterType<IRepository<Transaction, int>, Repository<Transaction, int>>(new HttpContextLifetimeManager<Transaction>())
                     .RegisterType<IRepository<Budget, int>, Repository<Budget, int>>(new HttpContextLifetimeManager<Budget>())
                     .RegisterType<IRepository<Category, int>, Repository<Category, int>>(new HttpContextLifetimeManager<Category>())
                     .RegisterType<IRepository<Category_Budget, int>, Repository<Category_Budget, int>>(new HttpContextLifetimeManager<Category_Budget>())
                     .RegisterType<IUserServices, UserServices>(new HttpContextLifetimeManager<UserServices>())
                     .RegisterType<IAccountServices, AccountServices>(new HttpContextLifetimeManager<AccountServices>())
                     .RegisterType<ITransactionServices, TransactionServices>(new HttpContextLifetimeManager<TransactionServices>())
                     .RegisterType<IBudgetServices, BudgetServices>(new HttpContextLifetimeManager<BudgetServices>())
                     .RegisterType<ICategoryServices, CategoryServices>(new HttpContextLifetimeManager<CategoryServices>())
                     .RegisterType<IUnitOfWork, UnitOfWork>(new HttpContextLifetimeManager<UnitOfWork>())
                     // .RegisterType<IDataStoreFactory, DataStoreFactory>(new HttpContextLifetimeManager<DataStoreFactory>())
                     .RegisterType<IDateTimeProvider, DateTimeWrapper>(new InjectionFactory(c => new DateTimeWrapper(() => DateTime.Now)))
                     .RegisterType<IDataStoreFactory>(new InjectionFactory(c => new DataStoreFactory((HttpContext.Current.Request.Cookies[cookieKey] != null) ? Convert.ToInt32(EncryptionHelpers.DecryptStringAES(HttpContext.Current.Request.Cookies[cookieKey].Value, sharedSecret)) : -1)))
                     .RegisterType<ICryptoProvider>(new InjectionFactory(c => new CryptoWrapper()))
                     .RegisterType<HttpContextBase>(new InjectionFactory(c => new HttpContextWrapper(HttpContext.Current)))
                     .RegisterType<ISiteConfiguration>(new InjectionFactory(c => new SiteConfiguration(sharedSecret, cookieKey)));

            var resolver = new UnityDependencyResolver(container);

            DependencyResolver.SetResolver(resolver);

            // Remove the XML formatter so that Web API actions return JSON by default
            var config = GlobalConfiguration.Configuration;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            // Web API controllers have a separate dependency resolver, which 
            // implements System.Web.Http.Dependencies.IDependencyResolver. 
            // The resolver we have is a System.Web.Mvc.IDependencyResolver, 
            // so we need to use an adapter class to get back a resolver of 
            // the correct type
            config.DependencyResolver = resolver.ToServiceResolver();

            //if (Convert.ToBoolean(ConfigurationManager.AppSettings["RegenerateDb"]))
            //    Database.SetInitializer<DataStore>(new DbInitializer());
            //else
            //    Database.SetInitializer<DataStore>(null);

            Database.SetInitializer<DataStore>(new MigrateDatabaseToLatestVersion<DataStore, MABMoney.Data.Migrations.Configuration>());

            MiniProfilerEF.Initialize();

            //Mapper.AddMapping<User, UserDTO>((s, d) => {
            //    d.UserID = s.UserID;
            //    d.Forename = "MAPPED!!!!";
            //    d.Accounts = s.Accounts.ToList().MapToList<AccountDTO>();
            //});

            var configuration = new MemcachedClientConfiguration();
            configuration.AddServer(ConfigurationManager.AppSettings["MEMCACHIER_SERVERS"]);
            configuration.Protocol = MemcachedProtocol.Binary;

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["MEMCACHIER_AUTH"]))
            {
                configuration.Authentication.Type = typeof(PlainTextAuthenticator);
                configuration.Authentication.Parameters["userName"] = ConfigurationManager.AppSettings["MEMCACHIER_USERNAME"];
                configuration.Authentication.Parameters["password"] = ConfigurationManager.AppSettings["MEMCACHIER_PASSWORD"];
                configuration.Authentication.Parameters["zone"] = "";
            }

            Global.Cache = new MemcachedClient(configuration);
        }
    }
}