using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;

namespace MABMoney.Web.Controllers
{
    public class BaseController : Controller
    {
        protected IUserServices _userServices;
        protected IAccountServices _accountServices;
        protected ICategoryServices _categoryServices;
        protected ITransactionServices _transactionServices;
        protected IBudgetServices _budgetServices;

        public BaseController(IUserServices userServices,
                              IAccountServices accountServices,
                              ICategoryServices categoryServices,
                              ITransactionServices transactionServices,
                              IBudgetServices budgetServices)
        {
            _userServices = userServices;
            _accountServices = accountServices;
            _categoryServices = categoryServices;
            _transactionServices = transactionServices;
            _budgetServices = budgetServices;
        }

    }
}
