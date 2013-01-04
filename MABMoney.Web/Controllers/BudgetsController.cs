using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;

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
            return View();
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
            return View();
        }

        //
        // POST: /Budget/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Budget/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Budget/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Budget/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Budget/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
