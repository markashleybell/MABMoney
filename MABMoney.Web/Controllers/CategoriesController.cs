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
using MABMoney.Data;
using MABMoney.Caching;

namespace MABMoney.Web.Controllers
{
    public class CategoriesController : BaseController
    {
        public CategoriesController(IUserServices userServices,
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
                Categories = _categoryServices.All().ToList()
            });
        }

        //
        // GET: /Account/Create
        [Authenticate]
        public ActionResult Create(ProfileViewModel profile)
        {
            return View(new CreateViewModel {
                CategoryTypes = DataHelpers.GetCategoryTypeSelectOptions(),
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices),
                RedirectAfterSubmitUrl = _url.Action("Index")
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
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
                model.CategoryTypes = DataHelpers.GetCategoryTypeSelectOptions();
                return View(model);
            }

            var dto = model.MapTo<CategoryDTO>();
            _categoryServices.Save(dto);

            // Clear the cache of anything that depends upon categories
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Category));

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        //
        // GET: /Account/Edit/5
        [Authenticate]
        public ActionResult Edit(ProfileViewModel profile, int id)
        {
            var model = _categoryServices.Get(id).MapTo<EditViewModel>();
            model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);model.CategoryTypes = DataHelpers.GetCategoryTypeSelectOptions();
            model.CategoryTypes = DataHelpers.GetCategoryTypeSelectOptions();
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
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
                model.CategoryTypes = DataHelpers.GetCategoryTypeSelectOptions();
                return View(model);
            }

            var dto = model.MapTo<CategoryDTO>();
            _categoryServices.Save(dto);

            // Clear the cache of anything that depends upon categories
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Category));

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        //
        // POST: /Account/Delete/5
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(ProfileViewModel profile, int id, string redirectAfterSubmitUrl)
        {
            _categoryServices.Delete(id);

            // Clear the cache of anything that depends upon categories
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Category));

            return Redirect(redirectAfterSubmitUrl);
        }
    }
}
