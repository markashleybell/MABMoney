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
using MABMoney.Web.Infrastructure;
using MABMoney.Web.Models;

namespace MABMoney.Web.Controllers
{
    public class CategoriesController : BaseController
    {
        public CategoriesController(IUserServices userServices,
                                    IAccountServices accountServices,
                                    ICategoryServices categoryServices,
                                    ITransactionServices transactionServices,
                                    IBudgetServices budgetServices,
                                    HttpContextBase context,
                                    ISiteConfiguration config): base(userServices,
                                                                     accountServices,
                                                                     categoryServices,
                                                                     transactionServices, 
                                                                     budgetServices,
                                                                     context,
                                                                     config) { }

        //
        // GET: /Account/
        [Authenticate]
        public ActionResult Index(ProfileViewModel profile)
        {
            return View(new IndexViewModel {
                Categories = _categoryServices.All(profile.UserID).ToList()
            });
        }

        //
        // GET: /Account/Details/5
        [Authenticate]
        public ActionResult Details(ProfileViewModel profile, int id)
        {
            return View();
        }

        //
        // GET: /Account/Create
        [Authenticate]
        public ActionResult Create(ProfileViewModel profile)
        {
            return View(new CreateViewModel {
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID)
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
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID);
                return View(model);
            }

            var dto = model.MapTo<CategoryDTO>();
            _categoryServices.Save(profile.UserID, dto);

            return RedirectToAction("Index");
        }

        //
        // GET: /Account/Edit/5
        [Authenticate]
        public ActionResult Edit(ProfileViewModel profile, int id)
        {
            var model = _categoryServices.Get(profile.UserID, id).MapTo<EditViewModel>();
            model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID);
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
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID);
                return View(model);
            }

            var dto = model.MapTo<CategoryDTO>();
            _categoryServices.Save(profile.UserID, dto);

            return RedirectToAction("Index");
        }

        //
        // POST: /Account/Delete/5
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(ProfileViewModel profile, int id)
        {
            _categoryServices.Delete(profile.UserID, id);
            return RedirectToAction("Index");
        }
    }
}
