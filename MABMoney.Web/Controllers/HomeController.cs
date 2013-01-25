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

        [Authenticate]
        public ActionResult Index(ProfileViewModel profile)
        {
            var user = _userServices.Get(profile.UserID);

            var account = _accountServices.Get(profile.UserID, user.Accounts.First().AccountID);
            var transactions = _transactionServices.All(profile.UserID).Where(x => x.Account_AccountID == account.AccountID).ToList();

            return View(new IndexViewModel { 
                Date = _dateProvider.Date,
                Account = account,
                Account_AccountID = account.AccountID,
                Transactions = transactions,
                IncomeCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, profile.UserID, account.AccountID, CategoryTypeDTO.Income),
                ExpenseCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, profile.UserID, account.AccountID, CategoryTypeDTO.Expense),
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID),
                Type = GetDefaultTransationTypeForAccount(account),
                Budget = _budgetServices.GetLatest(profile.UserID, account.AccountID)
            });
        }

        [Authenticate]
        [HttpPost]
        public ActionResult Index(ProfileViewModel profile, int account_accountId)
        {
            var user = _userServices.Get(profile.UserID);

            var account = _accountServices.Get(profile.UserID, account_accountId);
            var transactions = _transactionServices.All(profile.UserID).Where(x => x.Account_AccountID == account.AccountID).ToList();

            return View(new IndexViewModel
            {
                Date = _dateProvider.Date,
                Account = account,
                Account_AccountID = account.AccountID,
                Transactions = transactions,
                IncomeCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, profile.UserID, account.AccountID, CategoryTypeDTO.Income),
                ExpenseCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, profile.UserID, account.AccountID, CategoryTypeDTO.Expense),
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID),
                Type = GetDefaultTransationTypeForAccount(account),
                Budget = _budgetServices.GetLatest(profile.UserID, account.AccountID)
            });
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

            // Change to show index view directly so we can return to the correct 
            // account/tab without a ton of querystring params
            return View("Index", new IndexViewModel { 
                Date = _dateProvider.Date,
                Account = _accountServices.Get(profile.UserID, model.Account_AccountID),
                Transactions = _transactionServices.All(profile.UserID).Where(x => x.Account_AccountID == model.Account_AccountID).ToList(),
                IncomeCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, profile.UserID, model.Account_AccountID, CategoryTypeDTO.Income),
                ExpenseCategories = DataHelpers.GetCategorySelectOptions(_categoryServices, profile.UserID, model.Account_AccountID, CategoryTypeDTO.Expense),
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID),
                Type = model.Type,
                Budget = _budgetServices.GetLatest(profile.UserID, model.Account_AccountID)
            });
        }

        public ActionResult MainNavigation(ProfileViewModel profile)
        {
            return View(new MainNavigationViewModel { 
                Profile = profile,
                NetWorth = _accountServices.GetNetWorth(profile.UserID)
            });
        }
    }
}
