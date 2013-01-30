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
                              IDateTimeProvider dateProvider) : base(userServices,
                                                                     accountServices,
                                                                     categoryServices,
                                                                     transactionServices, 
                                                                     budgetServices,
                                                                     context,
                                                                     config,
                                                                     dateProvider) { }

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

            var account = _accountServices.Get(userId, ((accountId.HasValue) ? accountId.Value : user.Accounts.First().AccountID));

            var model = new IndexViewModel
            {
                Date = _dateProvider.Date,
                Account = account,
                Account_AccountID = account.AccountID,
                IncomeCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, userId, account.AccountID, CategoryTypeDTO.Income),
                ExpenseCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, userId, account.AccountID, CategoryTypeDTO.Expense),
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, userId),
                Type = GetDefaultTransationTypeForAccount(account)
            };

            var now = _dateProvider.Date;

            model.From = new DateTime(now.Year, now.Month, 1);
            model.To = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

            // Get latest budget, if there is one
            var latestBudget = _budgetServices.GetLatest(userId, account.AccountID);

            var transactions = _transactionServices.All(userId).Where(x => x.Account_AccountID == account.AccountID);

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
        public ActionResult Index(ProfileViewModel profile)
        {
            return View(GetModelData(profile.UserID, null));
        }

        [Authenticate]
        [HttpPost]
        public ActionResult Index(ProfileViewModel profile, int account_accountId)
        {
            return View(GetModelData(profile.UserID, account_accountId));
        }

        [Authenticate]
        [HttpPost]
        public ActionResult CreateTransaction(ProfileViewModel profile, IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Account = _accountServices.Get(profile.UserID, model.Account_AccountID);
                model.Transactions = _transactionServices.All(profile.UserID).Where(x => x.Account_AccountID == model.Account_AccountID).ToList();
                model.IncomeCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, profile.UserID, model.Account_AccountID, CategoryTypeDTO.Income);
                model.ExpenseCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, profile.UserID, model.Account_AccountID, CategoryTypeDTO.Expense);
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID);

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
            _transactionServices.Save(profile.UserID, dto);

            ModelState.Remove("Date");
            ModelState.Remove("Category_CategoryID");
            ModelState.Remove("Description");
            ModelState.Remove("Amount");

            var displayModel = GetModelData(profile.UserID, model.Account_AccountID);
            displayModel.Type = model.Type;

            return View("Index", displayModel);
        }

        public ActionResult MainNavigation(ProfileViewModel profile)
        {
            var accounts = _accountServices.All(profile.UserID).ToList();

            return View(new MainNavigationViewModel { 
                Profile = profile,
                Accounts = accounts,
                NetWorth = accounts.Sum(x => x.CurrentBalance)
            });
        }
    }
}
