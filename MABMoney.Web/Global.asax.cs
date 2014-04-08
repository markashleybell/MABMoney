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
using MABMoney.Web.Helpers;
using mab.lib.SimpleMapper;
using MABMoney.Services.DTO;
using System.Web.Optimization;
using System.Web.Configuration;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;
using StackExchange.Profiling.Storage;
using MABMoney.Caching;

namespace MABMoney.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(ProfileViewModel), new ProfileModelBinder());

            MiniProfiler.Settings.Storage = new SqlServerStorage(ConfigurationManager.ConnectionStrings["Profiler"].ConnectionString);
            MiniProfilerEF6.Initialize();

            var sharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
            var cookieKey = ConfigurationManager.AppSettings["CookieKey"];

            var externalDbConnectionString = ConfigurationManager.AppSettings["ExternalDbConnectionString"];

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
                     .RegisterType<ICategoryServices, CategoryServices>(new HttpContextLifetimeManager<CategoryServices>())
                     .RegisterType<IBudgetServices, BudgetServices>(new HttpContextLifetimeManager<BudgetServices>())
                     .RegisterType<IUnitOfWork, UnitOfWork>(new HttpContextLifetimeManager<UnitOfWork>())
                     .RegisterType<IDateTimeProvider, DateTimeProvider>(new InjectionFactory(c => new DateTimeProvider(() => DateTime.Now)))
                     .RegisterType<ICryptoProvider>(new InjectionFactory(c => new CryptoWrapper()))
                     .RegisterType<IUrlHelper>(new InjectionFactory(c => new UrlHelperAdapter(new UrlHelper(HttpContext.Current.Request.RequestContext))))
                     .RegisterType<IModelCache>(new InjectionFactory(c => new ModelCache()))
                     .RegisterType<IModelCacheConfiguration>(new InjectionFactory(c => new ModelCacheConfiguration()))
                     .RegisterType<IHttpContextProvider>(new InjectionFactory(c => new HttpContextProvider(new HttpContextWrapper(HttpContext.Current))))
                     .RegisterType<ISiteConfiguration>(new InjectionFactory(c => new SiteConfiguration()));

            if(externalDbConnectionString != null)
                container.RegisterType<IDataStoreFactory>(new InjectionFactory(c => new DataStoreFactory((HttpContext.Current.Request.Cookies[cookieKey] != null) ? Convert.ToInt32(EncryptionHelpers.DecryptStringAES(HttpContext.Current.Request.Cookies[cookieKey].Value, sharedSecret)) : -1, new DateTimeProvider(() => DateTime.Now), externalDbConnectionString)));
            else
                container.RegisterType<IDataStoreFactory>(new InjectionFactory(c => new DataStoreFactory((HttpContext.Current.Request.Cookies[cookieKey] != null) ? Convert.ToInt32(EncryptionHelpers.DecryptStringAES(HttpContext.Current.Request.Cookies[cookieKey].Value, sharedSecret)) : -1, new DateTimeProvider(() => DateTime.Now))));

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

            Database.SetInitializer<DataStore>(null);

            //if (externalDbConnectionString != null)
            //    Database.SetInitializer<DataStore>(new MigrateDatabaseToLatestVersionWithConnectionString<DataStore, MABMoney.Data.Migrations.Configuration>(externalDbConnectionString));
            //else
            //    Database.SetInitializer<DataStore>(new MigrateDatabaseToLatestVersion<DataStore, MABMoney.Data.Migrations.Configuration>());

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

        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)
            {
                MiniProfiler.Start();
            } 
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }
    }
}