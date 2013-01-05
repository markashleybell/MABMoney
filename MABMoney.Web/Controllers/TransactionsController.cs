using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;
using MABMoney.Web.Models.Transactions;
using MABMoney.Services.DTO;
using mab.lib.SimpleMapper;
using MABMoney.Web.Helpers;

namespace MABMoney.Web.Controllers
{
    public class TransactionsController : BaseController
    {
        public TransactionsController(IUserServices userServices,
                                      IAccountServices accountServices,
                                      ICategoryServices categoryServices,
                                      ITransactionServices transactionServices,
                                      IBudgetServices budgetServices) : base(userServices,
                                                                             accountServices,
                                                                             categoryServices,
                                                                             transactionServices, 
                                                                             budgetServices) { }

        //
        // GET: /Transaction/

        public ActionResult Index()
        {
            return View(new IndexViewModel {
                Transactions = _transactionServices.All().ToList()
            });
        }

        //
        // GET: /Transaction/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Transaction/Create

        public ActionResult Create()
        {
            return View(new CreateViewModel {
                Categories = DataHelpers.GetCategorySelectOptions(_categoryServices)
            });
        }

        //
        // POST: /Transaction/Create

        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices);
                return View(model);
            }

            var dto = model.MapTo<TransactionDTO>();
            _transactionServices.Save(dto);

            return RedirectToAction("Index");
        }

        //
        // GET: /Transaction/Edit/5

        public ActionResult Edit(int id)
        {
            var model = _transactionServices.Get(id).MapTo<EditViewModel>();
            model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices);
            return View(model);
        }

        //
        // POST: /Transaction/Edit/5

        [HttpPost]
        public ActionResult Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices);
                return View(model);
            }

            var dto = model.MapTo<TransactionDTO>();
            _transactionServices.Save(dto);

            return RedirectToAction("Index");
        }

        //
        // POST: /Transaction/Delete/5

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _transactionServices.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
