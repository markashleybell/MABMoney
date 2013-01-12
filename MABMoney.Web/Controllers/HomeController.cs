using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Domain;
using MABMoney.Services;
using MABMoney.Web.Models.Home;
using MABMoney.Web.Helpers;
using mab.lib.SimpleMapper;
using MABMoney.Services.DTO;

namespace MABMoney.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUserServices userServices,
                              IAccountServices accountServices,
                              ICategoryServices categoryServices,
                              ITransactionServices transactionServices,
                              IBudgetServices budgetServices) : base(userServices,
                                                                     accountServices,
                                                                     categoryServices,
                                                                     transactionServices, 
                                                                     budgetServices) { }

        public ActionResult Index()
        {
            var account = _accountServices.Get(1);
            var transactions = _transactionServices.All().Where(x => x.Account_AccountID == account.AccountID).ToList();

            return View(new IndexViewModel { 
                Account = account,
                Transactions = transactions,
                Categories = DataHelpers.GetCategorySelectOptions(_categoryServices),
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices),
                Type = TransactionType.Income
            });
        }

        [HttpPost]
        public ActionResult CreateTransaction(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Account = _accountServices.Get(model.Account_AccountID);
                model.Transactions = _transactionServices.All().Where(x => x.Account_AccountID == model.Account_AccountID).ToList();
                model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices);
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
                return View(model);
            }

            var dto = model.MapTo<TransactionDTO>();
            _transactionServices.Save(dto);

            return RedirectToAction("Index");
        }
    }
}
