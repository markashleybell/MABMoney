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
using MABMoney.Data;

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
                                 ISiteConfiguration config,
                                 IDateTimeProvider dateProvider,
                                 ICacheProvider cacheProvider) : base(userServices,
                                                                      accountServices,
                                                                      categoryServices,
                                                                      transactionServices, 
                                                                      budgetServices,
                                                                      context,
                                                                      config,
                                                                      dateProvider,
                                                                      cacheProvider) { }

        //
        // GET: /Budget/
        [AuthenticateAttribute]
        public ActionResult Index(ProfileViewModel profile)
        {
            return View(new IndexViewModel {
                Budgets = _budgetServices.All().ToList()
            });
        }

        //
        // GET: /Budget/Details/5
        [AuthenticateAttribute]
        public ActionResult Details(ProfileViewModel profile, int id)
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
        [Authenticate]
        public ActionResult Create(ProfileViewModel profile, int? id)
        {
            var categories = _categoryServices.All().Where(x => x.Type == CategoryTypeDTO.Expense);

            if(id.HasValue) 
                categories = categories.Where(x => x.Account_AccountID == id.Value);

            var now = _dateProvider.Now;
            
            return View(new CreateViewModel {
                Start = new DateTime(now.Year, now.Month, 1),
                End = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)),
                Account_AccountID = (id.HasValue) ? id.Value : 0,
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices),
                Categories = categories.ToList().Select(x => new Category_BudgetViewModel { 
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
        [Authenticate]
        public ActionResult Edit(ProfileViewModel profile, int id)
        {
            var dto = _budgetServices.Get(id);

            var model = dto.MapTo<EditViewModel>();
            model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
            model.Categories = dto.Category_Budgets.Where(x => x.Category_CategoryID != 0).Select(x => new Category_BudgetViewModel { 
                Name = x.Category.Name,
                Budget_BudgetID = x.Budget_BudgetID,
                Category_CategoryID = x.Category_CategoryID,
                Amount = x.Amount,
                Total = x.Total
            }).ToList();

            var categories = _categoryServices.All()
                                              .Where(x => x.Account_AccountID == dto.Account_AccountID && x.Type == CategoryTypeDTO.Expense && !model.Categories.Any(c => c.Category_CategoryID == x.CategoryID))
                                              .Select(x => new Category_BudgetViewModel
                                              {
                                                  Category_CategoryID = x.CategoryID,
                                                  Name = x.Name
                                              })
                                              .ToList();

            model.Categories.AddRange(categories);

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
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(ProfileViewModel profile, int id)
        {
            _budgetServices.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
