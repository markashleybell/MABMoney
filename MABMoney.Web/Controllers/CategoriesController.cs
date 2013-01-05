using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;
using MABMoney.Web.Models.Categories;
using MABMoney.Services.DTO;
using mab.lib.SimpleMapper;
using MABMoney.Web.Helpers;

namespace MABMoney.Web.Controllers
{
    public class CategoriesController : BaseController
    {
        public CategoriesController(IUserServices userServices,
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
                Categories = _categoryServices.All().ToList()
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
            return View(new CreateViewModel {
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices)
            });
        }

        //
        // POST: /Account/Create

        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
                return View(model);
            }

            var dto = model.MapTo<CategoryDTO>();
            _categoryServices.Save(dto);

            return RedirectToAction("Index");
        }

        //
        // GET: /Account/Edit/5

        public ActionResult Edit(int id)
        {
            var model = _categoryServices.Get(id).MapTo<EditViewModel>();
            model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
            return View(model);
        }

        //
        // POST: /Account/Edit/5

        [HttpPost]
        public ActionResult Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
                return View(model);
            }

            var dto = model.MapTo<CategoryDTO>();
            _categoryServices.Save(dto);

            return RedirectToAction("Index");
        }

        //
        // POST: /Account/Delete/5

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _categoryServices.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
