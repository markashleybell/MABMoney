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

namespace MABMoney.Web.Controllers
{
    public class AccountsController : BaseController
    {
        public AccountsController(IUserServices userServices,
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

        //
        // GET: /Account/
        [Authenticate]
        public ActionResult Index()
        {
            return View(new IndexViewModel {
                Accounts = _accountServices.All().ToList()
            });
        }

        //
        // GET: /Account/Details/5
        [Authenticate]
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Account/Create
        [Authenticate]
        public ActionResult Create()
        {
            return View(new CreateViewModel { 
                User_UserID = 1
            });
        }

        //
        // POST: /Account/Create
        [Authenticate]
        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = model.MapTo<AccountDTO>();
            _accountServices.Save(dto);

            return RedirectToAction("Index");
        }

        //
        // GET: /Account/Edit/5
        [Authenticate]
        public ActionResult Edit(int id)
        {
            var model = _accountServices.Get(id).MapTo<EditViewModel>();
            return View(model);
        }

        //
        // POST: /Account/Edit/5
        [Authenticate]
        [HttpPost]
        public ActionResult Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = model.MapTo<AccountDTO>();
            _accountServices.Save(dto);

            return RedirectToAction("Index");
        }

        //
        // POST: /Account/Delete/5
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            _accountServices.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
