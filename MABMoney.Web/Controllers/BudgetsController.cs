using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;
using MABMoney.Web.Models.Budgets;
using MABMoney.Services.DTO;
using mab.lib.SimpleMapper;
using MABMoney.Web.Helpers;
using MABMoney.Web.Infrastructure;
using MABMoney.Web.Models;

namespace MABMoney.Web.Controllers
{
    public class BudgetsController : BaseController
    {
        public BudgetsController(IUserServices userServices,
                                 IAccountServices accountServices,
                                 ICategoryServices categoryServices,
                                 ITransactionServices transactionServices,
                                 IBudgetServices budgetServices,
                                 HttpContextBase context,
                                 ISiteConfiguration config) : base(userServices,
                                                                   accountServices,
                                                                   categoryServices,
                                                                   transactionServices, 
                                                                   budgetServices,
                                                                   context,
                                                                   config) { }

        //
        // GET: /Budget/
        [AuthenticateAttribute]
        public ActionResult Index(ProfileViewModel profile)
        {
            return View(new IndexViewModel {
                Budgets = _budgetServices.All(profile.UserID).ToList()
            });
        }

        //
        // GET: /Budget/Details/5
        [AuthenticateAttribute]
        public ActionResult Details(ProfileViewModel profile, int id)
        {
            var dto = _budgetServices.Get(profile.UserID, id);
            var model = dto.MapTo<DetailsViewModel>();
            model.Categories = dto.Category_Budgets.Select(x => new Category_BudgetViewModel { 
                Name = x.Category.Name,
                Budget_BudgetID = x.Budget_BudgetID,
                Category_CategoryID = x.Category_CategoryID,
                Amount = x.Amount,
                Total = x.Total
            }).ToList();

            return View(model);
        }

        //
        // GET: /Budget/Create
        [Authenticate]
        public ActionResult Create(ProfileViewModel profile)
        {
            return View(new CreateViewModel {
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID),
                Categories = _categoryServices.All(profile.UserID).ToList().Select(x => new Category_BudgetViewModel { 
                    Category_CategoryID = x.CategoryID,
                    Name = x.Name
                }).ToList()
            });
        }

        //
        // POST: /Budget/Create
        [Authenticate]
        [HttpPost]
        public ActionResult Create(ProfileViewModel profile, CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID);
                return View(model);
            }

            var dto = model.MapTo<BudgetDTO>();
            _budgetServices.Save(profile.UserID, dto);

            foreach (var category in model.Categories)
            {
                _budgetServices.SaveCategoryBudget(profile.UserID, new Category_BudgetDTO { 
                    Budget_BudgetID = dto.BudgetID,
                    Category_CategoryID = category.Category_CategoryID,
                    Amount = category.Amount
                });
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Budget/Edit/5
        [Authenticate]
        public ActionResult Edit(ProfileViewModel profile, int id)
        {
            var dto = _budgetServices.Get(profile.UserID, id);
            var model = dto.MapTo<EditViewModel>();
            model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID);
            model.Categories = dto.Category_Budgets.Select(x => new Category_BudgetViewModel { 
                Name = x.Category.Name,
                Budget_BudgetID = x.Budget_BudgetID,
                Category_CategoryID = x.Category_CategoryID,
                Amount = x.Amount
            }).ToList();
            return View(model);
        }

        //
        // POST: /Budget/Edit/5
        [Authenticate]
        [HttpPost]
        public ActionResult Edit(ProfileViewModel profile, EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID);
                return View(model);
            }

            var dto = model.MapTo<BudgetDTO>();
            _budgetServices.Save(profile.UserID, dto);

            foreach (var category in model.Categories)
            {
                _budgetServices.SaveCategoryBudget(profile.UserID, new Category_BudgetDTO {
                    Budget_BudgetID = dto.BudgetID,
                    Category_CategoryID = category.Category_CategoryID,
                    Amount = category.Amount
                });
            }

            return RedirectToAction("Index");
        }

        //
        // POST: /Budget/Delete/5
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(ProfileViewModel profile, int id)
        {
            _budgetServices.Delete(profile.UserID, id);
            return RedirectToAction("Index");
        }
    }
}
