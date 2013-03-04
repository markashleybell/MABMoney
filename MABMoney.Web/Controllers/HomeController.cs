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

            var account = _accountServices.Get(accountId.Value);

            if (account == null)
                return null;

            model.Account = account;
            model.Account_AccountID = account.AccountID;
            model.IncomeCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, account.AccountID, CategoryTypeDTO.Income);
            model.ExpenseCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, account.AccountID, CategoryTypeDTO.Expense);
            model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
            model.AccountsWithBalances = DataHelpers.GetAccountSelectOptions(_accountServices, true);
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

            var transactions = _transactionServices.All().Where(x => x.Account_AccountID == account.AccountID);

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

            return model;
        }

        [Authenticate]
        public ActionResult Index(ProfileViewModel profile, string state = null)
        {
            var model = new IndexViewModel();
            model.Date = _dateProvider.Now;

            if (state != null)
            {
                var decodedPageState = Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(state));
                var pageState = EncryptionHelpers.DecryptStringAES(decodedPageState, _config.SharedSecret).Split('-');

                model = GetModelData(model, profile.UserID, Convert.ToInt32(pageState[0]));

                if (model == null)
                    return RedirectToAction("Create", "Account");

                model.Type = (TransactionType)Enum.Parse(typeof(TransactionType), pageState[1]);
                model.Tab = (DashboardTab)Enum.Parse(typeof(DashboardTab), pageState[2]);

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

        [Authenticate]
        [HttpPost]
        public ActionResult Index(ProfileViewModel profile, int account_accountId)
        {
            var pageState = EncryptionHelpers.EncryptStringAES(account_accountId + "-" + TransactionType.Expense + "-" + DashboardTab.BudgetOrPaymentCalc, _config.SharedSecret);
            var encodedPageState = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(pageState));

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

            var pageState = EncryptionHelpers.EncryptStringAES(model.Account_AccountID + "-" + model.Type + "-" + model.Tab, _config.SharedSecret);
            var encodedPageState = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(pageState));

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

            var pageState = EncryptionHelpers.EncryptStringAES(model.Account_AccountID + "-" + model.Type + "-" + model.Tab, _config.SharedSecret);
            var encodedPageState = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(pageState));

            return RedirectToRoute("Home", new { state = encodedPageState });
        }

        public ActionResult Signup(ProfileViewModel profile)
        {
            return View(new SignupViewModel());
        }

        [HttpPost]
        public ActionResult Signup(ProfileViewModel profile, SignupViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = model.MapTo<UserDTO>();
            _userServices.Save(dto);

            // Encrypt the ID before storing it in a cookie
            var encryptedUserId = EncryptionHelpers.EncryptStringAES(dto.UserID.ToString(), _config.SharedSecret);
            _context.Response.Cookies.Add(new HttpCookie(_config.CookieKey, encryptedUserId));

            return RedirectToAction("Index");
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
