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

namespace MABMoney.Web.Controllers
{
    public class AccountsController : BaseController
    {
        public AccountsController(IUserServices userServices,
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
            return View(new CreateViewModel { 
                AccountTypes = DataHelpers.GetAccountTypeSelectOptions()
            });
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

            return RedirectToAction("Index");
        }

        //
        // GET: /Account/Edit/5
        [Authenticate]
        public ActionResult Edit(ProfileViewModel profile, int id)
        {
            var model = _accountServices.Get(id).MapTo<EditViewModel>();
            model.AccountTypes = DataHelpers.GetAccountTypeSelectOptions();
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

            return RedirectToAction("Index");
        }

        //
        // POST: /Account/Delete/5
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(ProfileViewModel profile, int id)
        {
            _accountServices.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
