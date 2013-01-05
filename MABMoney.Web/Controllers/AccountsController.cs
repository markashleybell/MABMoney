using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;
using MABMoney.Web.Models.Accounts;
using MABMoney.Services.DTO;
using mab.lib.SimpleMapper;

namespace MABMoney.Web.Controllers
{
    public class AccountsController : BaseController
    {
        public AccountsController(IUserServices userServices,
                                  IAccountServices accountServices,
                                  ICategoryServices categoryServices,
                                  ITransactionServices transactionServices,
                                  IBudgetServices budgetServices) : base(userServices,
                                                                         accountServices,
                                                                         categoryServices,
                                                                         transactionServices, 
                                                                         budgetServices) { }

        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View(new IndexViewModel {
                Accounts = _accountServices.All().ToList()
            });
        }

        //
        // GET: /Account/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Account/Create

        public ActionResult Create()
        {
            return View(new CreateViewModel());
        }

        //
        // POST: /Account/Create

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

        public ActionResult Edit(int id)
        {
            var model = _accountServices.Get(id).MapTo<EditViewModel>();
            return View(model);
        }

        //
        // POST: /Account/Edit/5

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

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _accountServices.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
