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
            var dto = _budgetServices.Get(id);
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

        public ActionResult Create()
        {
            return View(new CreateViewModel {
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices),
                Categories = _categoryServices.All().ToList().Select(x => new Category_BudgetViewModel { 
                    Category_CategoryID = x.CategoryID,
                    Name = x.Name
                }).ToList()
            });
        }

        //
        // POST: /Budget/Create

        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
                return View(model);
            }

            var dto = model.MapTo<BudgetDTO>();
            _budgetServices.Save(dto);

            foreach (var category in model.Categories)
            {
                _budgetServices.SaveCategoryBudget(new Category_BudgetDTO { 
                    Budget_BudgetID = dto.BudgetID,
                    Category_CategoryID = category.Category_CategoryID,
                    Amount = category.Amount
                });
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Budget/Edit/5

        public ActionResult Edit(int id)
        {
            var dto = _budgetServices.Get(id);
            var model = dto.MapTo<EditViewModel>();
            model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
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

        [HttpPost]
        public ActionResult Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
                return View(model);
            }

            var dto = model.MapTo<BudgetDTO>();
            _budgetServices.Save(dto);

            foreach (var category in model.Categories)
            {
                _budgetServices.SaveCategoryBudget(new Category_BudgetDTO {
                    Budget_BudgetID = dto.BudgetID,
                    Category_CategoryID = category.Category_CategoryID,
                    Amount = category.Amount
                });
            }

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
