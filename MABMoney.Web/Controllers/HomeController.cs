﻿using System;
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
using MABMoney.Web.Infrastructure;

namespace MABMoney.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUserServices userServices,
                              IAccountServices accountServices,
                              ICategoryServices categoryServices,
                              ITransactionServices transactionServices,
                              IBudgetServices budgetServices, 
                              HttpContextBase context) : base(userServices,
                                                              accountServices,
                                                              categoryServices,
                                                              transactionServices, 
                                                              budgetServices,
                                                              context) { }

        [Authenticate]
        public ActionResult Index()
        {
            var user = _userServices.Get(Convert.ToInt32(_context.Items["UserID"]));

            var account = _accountServices.Get(user.Accounts.First().AccountID);
            var transactions = _transactionServices.All().Where(x => x.Account_AccountID == account.AccountID).ToList();

            return View(new IndexViewModel { 
                Account = account,
                Account_AccountID = account.AccountID,
                Transactions = transactions,
                Categories = DataHelpers.GetCategorySelectOptions(_categoryServices),
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices),
                Type = TransactionType.Income
            });
        }

        [Authenticate]
        [HttpPost]
        public ActionResult Index(int account_accountId)
        {
            var user = _userServices.Get(Convert.ToInt32(_context.Items["UserID"]));

            var account = _accountServices.Get(account_accountId);
            var transactions = _transactionServices.All().Where(x => x.Account_AccountID == account.AccountID).ToList();

            return View(new IndexViewModel
            {
                Account = account,
                Account_AccountID = account.AccountID,
                Transactions = transactions,
                Categories = DataHelpers.GetCategorySelectOptions(_categoryServices),
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices),
                Type = TransactionType.Income
            });
        }

        [Authenticate]
        [HttpPost]
        public ActionResult CreateTransaction(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Account = _accountServices.Get(model.Account_AccountID);
                model.Transactions = _transactionServices.All().Where(x => x.Account_AccountID == model.Account_AccountID).ToList();
                model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices);
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
                return View("Index", model);
            }

            switch (model.Type)
            {
                case TransactionType.Income:
                    if (model.Amount < 0)
                        model.Amount *= -1;
                    break;
                case TransactionType.Expense:
                    if (model.Amount > 0)
                        model.Amount *= -1;
                    break;
            }

            var dto = model.MapTo<TransactionDTO>();
            _transactionServices.Save(dto);

            // Change to show index view directly so we can return to the correct 
            // account/tab without a ton of querystring params
            return RedirectToAction("Index");
        }
    }
}
