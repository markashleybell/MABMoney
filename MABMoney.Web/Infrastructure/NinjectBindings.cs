using MABMoney.Caching;
using MABMoney.Data;
using MABMoney.Domain;
using MABMoney.Services;
using MABMoney.Web.Helpers;
using Ninject.Modules;
using Ninject.Web.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MABMoney.Web.Infrastructure
{
    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            var enableProfiling = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableProfiling"]);
            var enableMigrations = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableMigrations"]);
            var enableCaching = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableCaching"]);

            var sharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
            var cookieKey = ConfigurationManager.AppSettings["CookieKey"];
            var externalDbConnectionString = ConfigurationManager.AppSettings["ExternalDbConnectionString"];
            var profilerConnectionString = ConfigurationManager.AppSettings["ProfilerDbConnectionString"];

            if (externalDbConnectionString != null)
            {
                Bind<IDataStoreFactory>().To<DataStoreFactory>()
                                         .InRequestScope()
                                         .WithConstructorArgument("userId", c => ((HttpContext.Current.Request.Cookies[cookieKey] != null) ? Convert.ToInt32(EncryptionHelpers.DecryptStringAES(HttpContext.Current.Request.Cookies[cookieKey].Value, sharedSecret)) : -1))
                                         .WithConstructorArgument("dateTimeServices", new DateTimeProvider(() => DateTime.Now))
                                         .WithConstructorArgument("connectionString", externalDbConnectionString);
            }
            else
            {
                Bind<IDataStoreFactory>().To<DataStoreFactory>()
                                         .InRequestScope()
                                         .WithConstructorArgument("userId", c => ((HttpContext.Current.Request.Cookies[cookieKey] != null) ? Convert.ToInt32(EncryptionHelpers.DecryptStringAES(HttpContext.Current.Request.Cookies[cookieKey].Value, sharedSecret)) : -1))
                                         .WithConstructorArgument("dateTimeServices", new DateTimeProvider(() => DateTime.Now));
            }

            // Caching
            Bind<IModelCacheConfiguration>().To<ModelCacheConfiguration>().InRequestScope();
            Bind<IModelCache>().To<ModelCache>().InRequestScope();
            Bind<ICachingHelpers>().To<CachingHelpers>()
                                   .InRequestScope()
                                   .WithConstructorArgument("userId", c => ((HttpContext.Current.Request.Cookies[cookieKey] != null) ? Convert.ToInt32(EncryptionHelpers.DecryptStringAES(HttpContext.Current.Request.Cookies[cookieKey].Value, sharedSecret)) : -1));

            // Repositories
            Bind<IRepository<User, int>>().To<Repository<User, int>>().InRequestScope();
            Bind<IRepository<Account, int>>().To<Repository<Account, int>>().InRequestScope();
            Bind<IRepository<Transaction, int>>().To<Repository<Transaction, int>>().InRequestScope();
            Bind<IRepository<Budget, int>>().To<Repository<Budget, int>>().InRequestScope();
            Bind<IRepository<Category, int>>().To<Repository<Category, int>>().InRequestScope();
            Bind<IRepository<Category_Budget, int>>().To<Repository<Category_Budget, int>>().InRequestScope();
            
            // Services
            Bind<ICategoryServices>().To<CategoryServices>().InRequestScope();

            if (enableCaching)
            {
                Bind<IUserServices>().To<CachingUserServices>().InRequestScope();
                Bind<IUserServices>().To<UserServices>().When(r => r.Target.Name == "nonCachingUserServices").InRequestScope();
                Bind<IAccountServices>().To<CachingAccountServices>().InRequestScope();
                Bind<IAccountServices>().To<AccountServices>().When(r => r.Target.Name == "nonCachingAccountServices").InRequestScope();
                Bind<IBudgetServices>().To<CachingBudgetServices>().InRequestScope();
                Bind<IBudgetServices>().To<BudgetServices>().When(r => r.Target.Name == "nonCachingBudgetServices").InRequestScope();
                Bind<ITransactionServices>().To<CachingTransactionServices>().InRequestScope();
                Bind<ITransactionServices>().To<TransactionServices>().When(r => r.Target.Name == "nonCachingTransactionServices").InRequestScope();
            }
            else
            {
                Bind<IUserServices>().To<UserServices>().InRequestScope();
                Bind<IAccountServices>().To<AccountServices>().InRequestScope();
                Bind<IBudgetServices>().To<BudgetServices>().InRequestScope();
                Bind<ITransactionServices>().To<TransactionServices>().InRequestScope();
            }
            
            // Unit of work
            Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();

            // Providers
            Bind<IHttpContextProvider>().To<HttpContextProvider>().InRequestScope().WithConstructorArgument("context", c => new HttpContextWrapper(HttpContext.Current));
            Bind<Func<DateTime>>().ToMethod(m => () => DateTime.Now).WhenInjectedInto<DateTimeProvider>();
            Bind<IDateTimeProvider>().To<DateTimeProvider>().InRequestScope();
            Bind<ICryptoProvider>().To<CryptoWrapper>().InRequestScope();
            Bind<IUrlHelper>().To<UrlHelperAdapter>().InRequestScope().WithConstructorArgument("context", c => new HttpContextWrapper(HttpContext.Current));

            // Configuration
            Bind<ISiteConfiguration>().To<SiteConfiguration>().InRequestScope();
        }
    }
}