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
using StackExchange.Profiling;
using MABMoney.Caching;

namespace MABMoney.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUserServices userServices,
                              IAccountServices accountServices,
                              ICategoryServices categoryServices,
                              ITransactionServices transactionServices,
                              IBudgetServices budgetServices,
                              IHttpContextProvider context,
                              ISiteConfiguration config,
                              IDateTimeProvider dateProvider,
                              IUrlHelper urlHelper,
                              IModelCache cache,
                              ICachingHelpers cachingHelpers) : base(userServices,
                                                                     accountServices,
                                                                     categoryServices,
                                                                     transactionServices, 
                                                                     budgetServices,
                                                                     context,
                                                                     config,
                                                                     dateProvider,
                                                                     urlHelper,
                                                                     cache,
                                                                     cachingHelpers) { }

        private TransactionType GetDefaultTransactionTypeForAccount(AccountDTO account)
        {
            var accountName = account.Name.ToUpper();

            // Try and work out if this is a savings/deposit account or a current account
            if (accountName.Contains("SAVING") || accountName.Contains("SAVER") || accountName.Contains("ISA"))
                return TransactionType.Income;

            return TransactionType.Expense;
        }

        private IndexViewModel GetModelData(IndexViewModel model, int userId, int? accountId)
        {
            var user = _userServices.Get(userId);

            // If no account ID has been passed in
            if (!accountId.HasValue)
            {
                var defaultAccount = user.Accounts.Where(x => x.Default).FirstOrDefault();

                // If there isn't a default account
                if (defaultAccount == null)
                {
                    // If the user doesn't yet have any accounts, redirect them to the add account screen
                    if (user.Accounts == null || user.Accounts.Count == 0)
                        accountId = 0;
                    else // Just get the first one
                        accountId = user.Accounts.First().AccountID;
                }
                else
                {
                    // Set the account ID to the ID of the default account
                    accountId = defaultAccount.AccountID;
                }
            }

            string debug = null;

            var account = user.Accounts.FirstOrDefault(x => x.AccountID == accountId.Value);

            if (account == null)
                return null;

            model.Account = account;
            model.Account_AccountID = account.AccountID;
            model.IncomeCategories = DataHelpers.GetCategorySelectOptions(user, account.AccountID, CategoryTypeDTO.Income);
            model.ExpenseCategories = DataHelpers.GetCategorySelectOptions(user, account.AccountID, CategoryTypeDTO.Expense);
            model.Accounts = DataHelpers.GetAccountSelectOptions(user);
            model.AccountsWithBalances = DataHelpers.GetAccountSelectOptions(user, true);
            model.Type = GetDefaultTransactionTypeForAccount(account);
            model.Debug = debug;
            model.BudgetCount = _budgetServices.GetBudgetCount(account.AccountID);

            // If it's a savings account it will not have the payment calc or the budget tab, so just set to income
            model.Tab = (account.Type == AccountTypeDTO.Savings) ? DashboardTab.Income : DashboardTab.BudgetOrPaymentCalc;
            model.SourceAccountID = account.AccountID;

            var now = _dateProvider.Now;

            model.From = new DateTime(now.Year, now.Month, 1);
            model.To = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

            // Get latest budget, if there is one
            var latestBudget = _budgetServices.GetLatest(account.AccountID);

            var transactions = _transactionServices.GetForAccount(account.AccountID);

            if (latestBudget != null)
            {
                transactions = transactions.Where(x => x.Date >= latestBudget.Start.AddDays(-7) && x.Date <= latestBudget.End);
                model.From = latestBudget.Start;
                model.To = latestBudget.End;
            }
            else
            {
                transactions = transactions.Where(x => x.Date >= model.From.AddDays(-7) && x.Date <= model.To);
            }

            model.Transactions = transactions.ToList();

            model.Budget = latestBudget;

            model.DefaultCardPaymentAmount = _config.Get<decimal>("DefaultCardPaymentAmount");
            model.DefaultCardInterestRate = _config.Get<decimal>("DefaultCardInterestRate");
            model.DefaultCardMinimumPayment = _config.Get<decimal>("DefaultCardMinimumPayment");

            var userCardPaymentAmount = _context.GetCookieValue<decimal>(_config.Get<string>("CookieKey") + "_" + account.AccountID + "_DefaultCardPaymentAmount");
            var userCardInterestRate = _context.GetCookieValue<decimal>(_config.Get<string>("CookieKey") + "_" + account.AccountID + "_DefaultCardInterestRate");
            var userCardMinimumPayment = _context.GetCookieValue<decimal>(_config.Get<string>("CookieKey") + "_" + account.AccountID + "_DefaultCardMinimumPayment");

            if (userCardPaymentAmount != 0)
            {
                model.DefaultCardPaymentAmount = userCardPaymentAmount;
                model.DefaultCardInterestRate = userCardInterestRate;
                model.DefaultCardMinimumPayment = userCardMinimumPayment;
            }

            return model;
        }

        [Authenticate]
        public ActionResult Index(ProfileViewModel profile, string state = null)
        {
            var model = new IndexViewModel();
            model.Date = _dateProvider.Now;

            if (state != null)
            {
                var pageState = EncryptionHelpers.DecodeReturnParameters(state);

                model = GetModelData(model, profile.UserID, Convert.ToInt32(pageState[0]));

                if (model == null)
                    return RedirectToAction("Create", "Account");

                model.Type = (TransactionType)Convert.ToInt32(pageState[1]);
                model.Tab = (DashboardTab)Convert.ToInt32(pageState[2]);

                // If it's a savings account it will not have the payment calc or the budget tab, so just set to income
                if (model.Account.Type == AccountTypeDTO.Savings && model.Tab == DashboardTab.BudgetOrPaymentCalc)
                    model.Tab = DashboardTab.Income;

                return View(model);
            }
            else
            {
                model = GetModelData(model, profile.UserID, null);

                if (model == null)
                    return RedirectToAction("Create", "Accounts");

                return View(model);
            }
        }

        // We deliberately don't authenticate this action because it doesn't do much and this way we avoid 
        // an unnecessary DB call every time the accounts drop-down is changed
        [HttpPost]
        public ActionResult Index(ProfileViewModel profile, int account_accountId)
        {
            var encodedPageState = EncryptionHelpers.EncodeReturnParameters(account_accountId, TransactionType.Expense, DashboardTab.BudgetOrPaymentCalc);
            return RedirectToRoute("Home", new { state = encodedPageState });
        }

        [Authenticate]
        [HttpPost]
        public ActionResult CreateTransaction(ProfileViewModel profile, IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                GetModelData(model, profile.UserID, model.Account_AccountID);

                return View("Index", model);
            }

            model.Tab = DashboardTab.BudgetOrPaymentCalc;

            switch (model.Type)
            {
                case TransactionType.Income:
                    if (model.Amount < 0)
                        model.Amount *= -1;
                    model.Tab = DashboardTab.Income;
                    break;
                case TransactionType.Expense:
                    if (model.Amount > 0)
                        model.Amount *= -1;
                    model.Tab = DashboardTab.Expenses;
                    break;
            }

            var dto = model.MapTo<TransactionDTO>();
            _transactionServices.Save(dto);

            // Update the transaction description history for this account
            var account = _accountServices.Get(model.Account_AccountID);
            account.TransactionDescriptionHistory.Add(dto.Description);
            _accountServices.Save(account);

            // Clear the cache of anything that depends upon transactions
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Transaction));
            // Clear the user because current balance comes from User.Accounts property
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.User));

            var encodedPageState = EncryptionHelpers.EncodeReturnParameters(model.Account_AccountID, model.Type, model.Tab);

            return RedirectToRoute("Home", new { state = encodedPageState });
        }

        [Authenticate]
        [HttpPost]
        public ActionResult CreateTransfer(ProfileViewModel profile, IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                GetModelData(model, profile.UserID, model.Account_AccountID);
                model.Tab = DashboardTab.Transfers;
                return View("Index", model);
            }

            model.Tab = DashboardTab.Transfers;
            var sourceAccount = _accountServices.Get(model.SourceAccountID);
            var destinationAccount =  _accountServices.Get(model.DestinationAccountID);

            // Do Transfer
            var guid = Guid.NewGuid().ToString();

            // Make sure the amount is not negative
            if(model.Amount < 0)
                model.Amount *= -1;

            var sourceTransaction = new TransactionDTO {
                TransferGUID = guid,
                Account_AccountID = model.SourceAccountID,
                Date = model.Date,
                Amount = (model.Amount * -1),
                Description = "Transfer to " + destinationAccount.Name
            };

            var destinationTransaction = new TransactionDTO
            {
                TransferGUID = guid,
                Account_AccountID = model.DestinationAccountID,
                Date = model.Date,
                Amount = model.Amount,
                Description = "Transfer from " + sourceAccount.Name
            };

            _transactionServices.Save(sourceTransaction);
            _transactionServices.Save(destinationTransaction);

            // Clear the cache of anything that depends upon transactions
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Transaction));
            // Clear the user because current balance comes from User.Accounts property
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.User));

            var encodedPageState = EncryptionHelpers.EncodeReturnParameters(model.Account_AccountID, model.Type, model.Tab);

            return RedirectToRoute("Home", new { state = encodedPageState });
        }

        public ActionResult MainNavigation(ProfileViewModel profile)
        {
            string debug = null;

            var user = _userServices.GetMinimal(profile.UserID);

            var accounts = _accountServices.All().ToList();

            return View(new MainNavigationViewModel { 
                UserID = (user != null) ? user.UserID : 0,
                UserEmail = (user != null) ? user.Email : null,
                IsAdmin = (user != null) ? user.IsAdmin : false,
                Accounts = accounts,
                NetWorth = accounts.Sum(x => x.CurrentBalance),
                Debug = debug
            });
        }

        public ActionResult PingTarget()
        {
            var user = _userServices.Get(1);

            return View();
        }

        #region Caching

        public ActionResult ClearCacheContents(ProfileViewModel profile, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _cache.Clear();
                return Content("CLEARED ALL CACHE");
            }
            else
            {
                _cache.Remove(id);
                return Content("CLEARED CACHE " + id);
            }
        }

        public ActionResult ShowCacheContents(ProfileViewModel profile)
        {
            // We filter on Site Key because newer versions of MVC seem to stuff a load of other data into the memory cache 
            // which we're not interested in (break on the line below and inspect _cache.BaseCache to see what I mean...)
            return View(_cache.Items.Where(x => x.Key.StartsWith(_config.Get<string>("CookieKey"))).OrderByDescending(x => x.Hits).ToList());
        }

        public ActionResult InvalidateCache(ProfileViewModel profile, string id)
        {
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(id));
            return Content("INVALIDATED DEPENDENCY: " + id);
        }

        #endregion
    }
}
