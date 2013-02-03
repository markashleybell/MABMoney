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
using MABMoney.Web.Models;
using System.Text;
using MABMoney.Data;

namespace MABMoney.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUserServices userServices,
                              IAccountServices accountServices,
                              ICategoryServices categoryServices,
                              ITransactionServices transactionServices,
                              IBudgetServices budgetServices, 
                              HttpContextBase context,
                              ISiteConfiguration config,
                              IDateTimeProvider dateProvider,
                              ICacheProvider cacheProvider) : base(userServices,
                                                                   accountServices,
                                                                   categoryServices,
                                                                   transactionServices, 
                                                                   budgetServices,
                                                                   context,
                                                                   config,
                                                                   dateProvider,
                                                                   cacheProvider) { }

        private TransactionType GetDefaultTransationTypeForAccount(AccountDTO account)
        {
            var accountName = account.Name.ToUpper();

            // Try and work out if this is a savings/deposit account or a current account
            if (accountName.Contains("SAVING") || accountName.Contains("SAVER") || accountName.Contains("ISA"))
                return TransactionType.Income;

            return TransactionType.Expense;
        }

        private IndexViewModel GetModelData(int userId, int? accountId)
        {
            var user = _userServices.Get(userId);

            string debug = null;

            //var key = "ACCOUNT_DTO_" + userId;
            //AccountDTO account = _cacheProvider.Get<AccountDTO>(key);

            //if (account == null)
            //{
            //    account = _accountServices.Get(((accountId.HasValue) ? accountId.Value : user.Accounts.First().AccountID));
            //    var set = _cacheProvider.Set(key, account);

            //    debug = key + ": NOT IN CACHE, " + ((set) ? "SET" : "FAILED TO SET");
            //}
            //else
            //{
            //    debug = key + ": RETRIEVED FROM CACHE";
            //}

            // If no account ID has been passed in
            if (!accountId.HasValue)
            {
                var defaultAccount = user.Accounts.Where(x => x.Default).FirstOrDefault();

                // If there isn't a default account
                if (defaultAccount == null)
                {
                    // Just get the first one
                    accountId = user.Accounts.First().AccountID;
                }
                else
                {
                    // Set the account ID to the ID of the default account
                    accountId = defaultAccount.AccountID;
                }
            }

            var account = _accountServices.Get(accountId.Value);

            var model = new IndexViewModel
            {
                Date = _dateProvider.Now,
                Account = account,
                Account_AccountID = account.AccountID,
                IncomeCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, account.AccountID, CategoryTypeDTO.Income),
                ExpenseCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, account.AccountID, CategoryTypeDTO.Expense),
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices),
                Type = GetDefaultTransationTypeForAccount(account),
                Debug = debug
            };

            var now = _dateProvider.Now;

            model.From = new DateTime(now.Year, now.Month, 1);
            model.To = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

            // Get latest budget, if there is one
            var latestBudget = _budgetServices.GetLatest(account.AccountID);

            var transactions = _transactionServices.All().Where(x => x.Account_AccountID == account.AccountID);

            if (latestBudget != null)
            {
                transactions = transactions.Where(x => x.Date >= latestBudget.Start && x.Date <= latestBudget.End);
                model.From = latestBudget.Start;
                model.To = latestBudget.End;
            }

            model.Transactions = transactions.ToList();
            model.Budget = latestBudget;

            return model;
        }

        [Authenticate]
        public ActionResult Index(ProfileViewModel profile, string state = null)
        {
            if (state != null)
            {
                var decodedPageState = Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(state));
                var pageState = EncryptionHelpers.DecryptStringAES(decodedPageState, _config.SharedSecret).Split('-');

                var model = GetModelData(profile.UserID, Convert.ToInt32(pageState[0]));

                model.Type = (TransactionType)Enum.Parse(typeof(TransactionType), pageState[1]);

                return View(model);
            }

            return View(GetModelData(profile.UserID, null));
        }

        [Authenticate]
        [HttpPost]
        public ActionResult Index(ProfileViewModel profile, int account_accountId)
        {
            var pageState = EncryptionHelpers.EncryptStringAES(account_accountId + "-" + TransactionType.Expense, _config.SharedSecret);
            var encodedPageState = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(pageState));

            return RedirectToRoute("Home", new { state = encodedPageState });
        }

        [Authenticate]
        [HttpPost]
        public ActionResult CreateTransaction(ProfileViewModel profile, IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Account = _accountServices.Get(model.Account_AccountID);
                model.Transactions = _transactionServices.All().Where(x => x.Account_AccountID == model.Account_AccountID).ToList();
                model.IncomeCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, model.Account_AccountID, CategoryTypeDTO.Income);
                model.ExpenseCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, model.Account_AccountID, CategoryTypeDTO.Expense);
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


            var pageState = EncryptionHelpers.EncryptStringAES(model.Account_AccountID + "-" + model.Type, _config.SharedSecret);
            var encodedPageState = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(pageState));

            return RedirectToRoute("Home", new { state = encodedPageState });
        }

        public ActionResult MainNavigation(ProfileViewModel profile)
        {
            var accounts = _accountServices.All().ToList();

            return View(new MainNavigationViewModel { 
                Profile = profile,
                Accounts = accounts,
                NetWorth = accounts.Sum(x => x.CurrentBalance)
            });
        }
    }
}
