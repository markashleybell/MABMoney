using MABMoney.Caching;
using MABMoney.Services;
using MABMoney.Web.Infrastructure;
using System.Web.Mvc;

namespace MABMoney.Web.Controllers
{
    [MABMoney.Web.Infrastructure.RequireHttps]
    public class BaseController : Controller
    {
        protected IUserServices _userServices;
        protected IAccountServices _accountServices;
        protected ICategoryServices _categoryServices;
        protected ITransactionServices _transactionServices;
        protected IBudgetServices _budgetServices;
        protected IHttpContextProvider _context;
        protected ISiteConfiguration _config;
        protected IUrlHelper _url;
        protected IModelCache _cache;
        protected ICachingHelpers _cachingHelpers;

        public BaseController(IUserServices userServices,
                              IAccountServices accountServices,
                              ICategoryServices categoryServices,
                              ITransactionServices transactionServices,
                              IBudgetServices budgetServices,
                              IHttpContextProvider context,
                              ISiteConfiguration config,
                              IUrlHelper urlHelper,
                              IModelCache cache,
                              ICachingHelpers cachingHelpers)
        {
            _userServices = userServices;
            _accountServices = accountServices;
            _categoryServices = categoryServices;
            _transactionServices = transactionServices;
            _budgetServices = budgetServices;
            _context = context;
            _config = config;
            _url = urlHelper;
            _cache = cache;
            _cachingHelpers = cachingHelpers;
        }
    }
}
