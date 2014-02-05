﻿using System;
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
                                  IDateTimeProvider dateProvider,
                                  IUrlHelper urlHelper) : base(userServices,
                                                               accountServices,
                                                               categoryServices,
                                                               transactionServices, 
                                                               budgetServices,
                                                               context,
                                                               config,
                                                               dateProvider,
                                                               urlHelper) { }

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

            var pageState = EncryptionHelpers.EncryptStringAES(dto.AccountID + "-" + MABMoney.Web.Models.Home.TransactionType.Expense + "-" + MABMoney.Web.Models.Home.DashboardTab.BudgetOrPaymentCalc, _config.Get<string>("SharedSecret"));
            var encodedPageState = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(pageState));

            // TODO: This doesn't respect RedirectAfterSubmitUrl, should it?
            return RedirectToRoute("Home", new { state = encodedPageState });
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

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        //
        // POST: /Account/Delete/5
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(ProfileViewModel profile, int id, string redirectAfterSubmitUrl)
        {
            _accountServices.Delete(id);
            return Redirect(redirectAfterSubmitUrl);
        }

        //
        // POST: /Account/Delete/5
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
