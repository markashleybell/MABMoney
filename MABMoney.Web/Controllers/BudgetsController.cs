using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;
using MABMoney.Web.Models.Budgets;
using MABMoney.Services.DTO;
using mab.lib.SimpleMapper;

namespace MABMoney.Web.Controllers
{
    public class BudgetsController : BaseController
    {
        public BudgetsController(IUserServices userServices,
                                 IAccountServices accountServices,
                                 ICategoryServices categoryServices,
                                 ITransactionServices transactionServices,
                                 IBudgetServices budgetServices) : base(userServices,
                                                                        accountServices,
                                                                        categoryServices,
                                                                        transactionServices, 
                                                                        budgetServices) { }

        //
        // GET: /Budget/

        public ActionResult Index()
        {
            return View(new IndexViewModel {
                Budgets = _budgetServices.All().ToList()
            });
        }

        //
        // GET: /Budget/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Budget/Create

        public ActionResult Create()
        {
            return View(new CreateViewModel());
        }

        //
        // POST: /Budget/Create

        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = model.MapTo<BudgetDTO>();
            _budgetServices.Save(dto);

            return RedirectToAction("Index");
        }

        //
        // GET: /Budget/Edit/5

        public ActionResult Edit(int id)
        {
            var model = _budgetServices.Get(id).MapTo<EditViewModel>();
            return View(model);
        }

        //
        // POST: /Budget/Edit/5

        [HttpPost]
        public ActionResult Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = model.MapTo<BudgetDTO>();
            _budgetServices.Save(dto);

            return RedirectToAction("Index");
        }

        //
        // POST: /Budget/Delete/5

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _budgetServices.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
