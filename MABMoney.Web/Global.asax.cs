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

namespace MABMoney.Web
{
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

            // Set up object mappings for Unity DI
            var container = new UnityContainer();
 
            // TODO: Look into object lifetime management
            container.RegisterType<IRepository<User, int>, Repository<User, int>>()
                     .RegisterType<IRepository<Account, int>, Repository<Account, int>>()
                     .RegisterType<IUserServices, UserServices>()
                     .RegisterType<IUnitOfWork, UnitOfWork>()
                     .RegisterType<IDataStoreFactory, DataStoreFactory>();

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

            Database.SetInitializer<DataStore>(new DbInitializer());
        }
    }

    public class DbInitializer : DropCreateDatabaseIfModelChanges<DataStore>
    {
        protected override void Seed(DataStore context)
        {
            var users = new List<User> {
                new User {
                    Forename = "Mark",
                    Surname = "Bell",
                    Email = "me@markashleybell.com"
                }
            };

            foreach (var user in users)
                context.Users.Add(user);

            var accounts = new List<Account> {
                new Account {
                    Name = "Current",
                    User = users[0]
                }
            };

            foreach (var account in accounts)
                context.Accounts.Add(account);
        }
    }
}