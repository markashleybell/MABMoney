using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;
using MABMoney.Web.Models.Accounts;
using MABMoney.Services.DTO;
using mab.lib.SimpleMapper;
using MABMoney.Web.Infrastructure;
using MABMoney.Web.Models;
using MABMoney.Web.Helpers;
using MABMoney.Data;
using System.Text;
using MABMoney.Caching;

namespace MABMoney.Web.Controllers
{
    public class AccountsController : BaseController
    {
        public AccountsController(IUserServices userServices,
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

        //
        // GET: /Account/
        [Authenticate]
        public ActionResult Index(ProfileViewModel profile)
        {
            return View(new IndexViewModel {
                Accounts = _accountServices.All().ToList()
            });
        }

        //
        // GET: /Account/Create
        [Authenticate]
        public ActionResult Create(ProfileViewModel profile)
        {
            var accounts = _accountServices.All();

            var model = new CreateViewModel {
                AccountTypes = DataHelpers.GetAccountTypeSelectOptions(),
                Default = (accounts.Count() == 0) ? true : false,
                IncludeInNetWorth = true,
                RedirectAfterSubmitUrl = _url.Action("Index")
            };

            if (accounts.Count() == 0)
                model.Type = AccountTypeDTO.Current;

            return View(model);
        }

        //
        // POST: /Account/Create
        [Authenticate]
        [HttpPost]
        public ActionResult Create(ProfileViewModel profile, CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AccountTypes = DataHelpers.GetAccountTypeSelectOptions();
                return View(model);
            }

            var dto = model.MapTo<AccountDTO>();
            _accountServices.Save(dto);

            var encodedPageState = EncryptionHelpers.EncodeReturnParameters(dto.AccountID, MABMoney.Web.Models.Home.TransactionType.Expense, MABMoney.Web.Models.Home.DashboardTab.BudgetOrPaymentCalc);

            // Clear the user because current balance comes from User.Accounts property
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.User));
            // Clear the account cache so redirect is valid
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Account));

            // TODO: This doesn't respect RedirectAfterSubmitUrl, should it?
            return RedirectToRoute("Home-State", new { state = encodedPageState });
        }

        //
        // GET: /Account/Edit/5
        [Authenticate]
        public ActionResult Edit(ProfileViewModel profile, int id)
        {
            var model = _accountServices.Get(id).MapTo<EditViewModel>();
            model.AccountTypes = DataHelpers.GetAccountTypeSelectOptions();
            model.RedirectAfterSubmitUrl = _url.Action("Index");
            return View(model);
        }

        //
        // POST: /Account/Edit/5
        [Authenticate]
        [HttpPost]
        public ActionResult Edit(ProfileViewModel profile, EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AccountTypes = DataHelpers.GetAccountTypeSelectOptions();
                return View(model);
            }

            var dto = model.MapTo<AccountDTO>();
            _accountServices.Save(dto);

            // Clear the user because current balance comes from User.Accounts property
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.User));
            // Clear the account cache
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Account));

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        //
        // POST: /Account/Delete/5
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(ProfileViewModel profile, int id, string redirectAfterSubmitUrl)
        {
            _accountServices.Delete(id);

            // Clear the user because current balance comes from User.Accounts property
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.User));
            // Clear the cached account list
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Account));

            return Redirect(redirectAfterSubmitUrl);
        }

        [Authenticate]
        [HttpPost]
        public ActionResult GetTransactionDescriptionHistory(ProfileViewModel profile, string query, int? id)
        {
            if (!id.HasValue)
                return Json(new string[0]);

            var account = _accountServices.Get(id.Value);

            return Json(account.TransactionDescriptionHistory.ToArray());
        }
    }
}
