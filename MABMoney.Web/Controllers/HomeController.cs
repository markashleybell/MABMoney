﻿using mab.lib.SimpleMapper;
using MABMoney.Caching;
using MABMoney.Services;
using MABMoney.Services.DTO;
using MABMoney.Web.Helpers;
using MABMoney.Web.Infrastructure;
using MABMoney.Web.Models;
using MABMoney.Web.Models.Home;
using System;
using System.Linq;
using System.Web.Mvc;

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
                              IUrlHelper urlHelper,
                              IModelCache cache,
                              ICachingHelpers cachingHelpers) : base(userServices,
                                                                     accountServices,
                                                                     categoryServices,
                                                                     transactionServices, 
                                                                     budgetServices,
                                                                     context,
                                                                     config,
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
            var accounts = _accountServices.All();

            // If no account ID has been passed in
            if (!accountId.HasValue)
            {
                var defaultAccount = accounts.Where(x => x.Default).FirstOrDefault();

                // If there isn't a default account
                if (defaultAccount == null)
                {
                    // If the user doesn't yet have any accounts, redirect them to the add account screen
                    if (accounts == null || accounts.Count() == 0)
                        accountId = 0;
                    else // Just get the first one
                        accountId = accounts.First().AccountID;
                }
                else
                {
                    // Set the account ID to the ID of the default account
                    accountId = defaultAccount.AccountID;
                }
            }

            string debug = null;

            var account = accounts.FirstOrDefault(x => x.AccountID == accountId.Value);

            if (account == null)
                return null;

            model.Account = account;
            model.Account_AccountID = account.AccountID;
            model.SourceAccountID = account.AccountID;

            var categories = _categoryServices.All().Where(a => a.Account_AccountID == account.AccountID);

            model.IncomeCategories = categories.Where(c => c.Type == CategoryTypeDTO.Income)
                                               .Select(c => new SelectListItem {
                                                   Value = c.CategoryID.ToString(),
                                                   Text = c.Name
                                               }).AsQueryable();

            model.ExpenseCategories = categories.Where(c => c.Type == CategoryTypeDTO.Expense)
                                                .Select(c => new SelectListItem {
                                                    Value = c.CategoryID.ToString(),
                                                    Text = c.Name
                                                }).AsQueryable();

            model.Accounts = accounts.Select(x => new SelectListItem {
                Value = x.AccountID.ToString(),
                Text = x.Name
            }).AsQueryable();

            model.AccountsWithBalances = accounts.Select(x => new SelectListItem {
                Value = x.AccountID.ToString(),
                Text = x.Name + " / " + x.CurrentBalance
            }).AsQueryable();

            model.Type = GetDefaultTransactionTypeForAccount(account);
            model.Debug = debug;
            model.BudgetCount = _budgetServices.GetBudgetCount(account.AccountID);

            // If it's a savings account it will not have the payment calc or the budget tab, so just set to income
            model.Tab = (account.Type == AccountTypeDTO.Savings) ? DashboardTab.Income : DashboardTab.BudgetOrPaymentCalc;
            model.SourceAccountID = account.AccountID;

            var now = DateTime.Now;

            model.From = new DateTime(now.Year, now.Month, 1);
            model.To = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

            // Get latest budget, if there is one
            var latestBudget = _budgetServices.GetLatest(now, account.AccountID);

            if (latestBudget != null)
            {
                // Show transactions made during the latest budget period
                model.Transactions = _transactionServices.GetForAccount(account.AccountID, latestBudget.Start.AddDays(-7), latestBudget.End).ToList();
                model.From = latestBudget.Start;
                model.To = latestBudget.End;
            }
            else
            {
                // If there is no budget at all, for any account, just show transactions from the first of the current month
                model.Transactions = _transactionServices.GetForAccount(account.AccountID, model.From.AddDays(-7), model.To).ToList();
            }

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
            model.Date = DateTime.Now;

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
            return RedirectToRoute("Home-State", new { state = encodedPageState });
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

            // Clear the cache of anything that depends upon transactions
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Transaction));
            // Clear the user because current balance comes from User.Accounts property
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.User));

            var encodedPageState = EncryptionHelpers.EncodeReturnParameters(model.Account_AccountID, model.Type, model.Tab);

            return RedirectToRoute("Home-State", new { state = encodedPageState });
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
                Category_CategoryID = model.Category_CategoryID,
                Amount = (model.Amount * -1),
                Description = "Transfer to " + destinationAccount.Name,
                Note = model.Note
            };

            var destinationTransaction = new TransactionDTO {
                TransferGUID = guid,
                Account_AccountID = model.DestinationAccountID,
                Date = model.Date,
                // Don't set the category of the destination because we can't know what it should be
                // Category_CategoryID = model.Category_CategoryID,
                Amount = model.Amount,
                Description = "Transfer from " + sourceAccount.Name,
                Note = model.Note
            };

            _transactionServices.Save(sourceTransaction);
            _transactionServices.Save(destinationTransaction);

            // Clear the cache of anything that depends upon transactions
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Transaction));
            // Clear the user because current balance comes from User.Accounts property
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.User));

            var encodedPageState = EncryptionHelpers.EncodeReturnParameters(model.Account_AccountID, model.Type, model.Tab);

            return RedirectToRoute("Home-State", new { state = encodedPageState });
        }

        public ActionResult MainNavigation(ProfileViewModel profile)
        {
            string debug = null;

            var accounts = _accountServices.All().ToList();

            var netWorthAccounts = accounts.Where(x => x.IncludeInNetWorth).ToList();
            var nonNetWorthAccounts = accounts.Where(x => !x.IncludeInNetWorth).ToList();

            return View(new MainNavigationViewModel { 
                UserID = (profile != null) ? profile.UserID : 0,
                UserEmail = (profile != null) ? profile.Email : null,
                Accounts = accounts,
                NetWorthAccounts = netWorthAccounts,
                NonNetWorthAccounts = nonNetWorthAccounts,
                NetWorth = netWorthAccounts.Sum(x => x.CurrentBalance),
                Debug = debug
            });
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
