using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;
using MABMoney.Web.Infrastructure;
using MABMoney.Data;

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
        protected IDateTimeProvider _dateProvider;
        protected ICacheProvider _cacheProvider;

        protected IUrlHelper _url;

        public BaseController(IUserServices userServices,
                              IAccountServices accountServices,
                              ICategoryServices categoryServices,
                              ITransactionServices transactionServices,
                              IBudgetServices budgetServices,
                              IHttpContextProvider context,
                              ISiteConfiguration config,
                              IDateTimeProvider dateProvider,
                              ICacheProvider cacheProvider,
                              IUrlHelper urlHelper)
        {
            _userServices = userServices;
            _accountServices = accountServices;
            _categoryServices = categoryServices;
            _transactionServices = transactionServices;
            _budgetServices = budgetServices;
            _context = context;
            _config = config;
            _dateProvider = dateProvider;
            _cacheProvider = cacheProvider;

            _url = urlHelper;
        }

    }
}
