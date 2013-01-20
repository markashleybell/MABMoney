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

            ModelBinders.Binders.Add(typeof(ProfileViewModel), new ProfileModelBinder());

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
                     .RegisterType<IDataStoreFactory, DataStoreFactory>(new HttpContextLifetimeManager<DataStoreFactory>())
                     .RegisterType<ICryptoWrapper>(new InjectionFactory(c => new CryptoWrapper()))
                     .RegisterType<HttpContextBase>(new InjectionFactory(c => new HttpContextWrapper(HttpContext.Current)))
                     .RegisterType<ISiteConfiguration>(new InjectionFactory(c => new SiteConfiguration(ConfigurationManager.AppSettings["SharedSecret"])));

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

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["RegenerateDb"]))
                Database.SetInitializer<DataStore>(new DbInitializer());
            else
                Database.SetInitializer<DataStore>(null);

            MiniProfilerEF.Initialize();
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
                    Email = "me@markashleybell.com",
                    Password = "ALZByiJAcShmiQpEue7DR0g+RZYSWiSXJoF3VxFck96CEhW8SZakdXaJ+1PDgqGoXw==" // test123
                }
            };

            foreach (var user in users)
                context.Users.Add(user);

            var accounts = new List<Account> {
                new Account {
                    Name = "Current",
                    User = users[0],
                    StartingBalance = 0
                },
                new Account {
                    Name = "Savings",
                    User = users[0],
                    StartingBalance = 0
                },
                new Account {
                    Name = "Credit Card",
                    User = users[0],
                    StartingBalance = 0
                }
            };

            foreach (var account in accounts)
                context.Accounts.Add(account);

            var categories = new List<Category> {
                new Category {
                    Account = accounts[0],
                    Name = "Salary",
                    Type = CategoryType.Income
                },
                new Category {
                    Account = accounts[0],
                    Name = "Rent",
                    Type = CategoryType.Expense
                },
                new Category {
                    Account = accounts[0],
                    Name = "Food",
                    Type = CategoryType.Expense
                },
                new Category {
                    Account = accounts[0],
                    Name = "Fuel",
                    Type = CategoryType.Expense
                }
            };

            foreach (var category in categories)
                context.Categories.Add(category);
        }
    }
}