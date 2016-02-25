using mab.lib.SimpleMapper;
using MABMoney.Caching;
using MABMoney.Services;
using MABMoney.Services.DTO;
using MABMoney.Web.Helpers;
using MABMoney.Web.Infrastructure;
using MABMoney.Web.Models;
using MABMoney.Web.Models.Budgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MABMoney.Web.Controllers
{
    public class BudgetsController : BaseController
    {
        public BudgetsController(IUserServices userServices,
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
        // GET: /Budget/
        [AuthenticateAttribute]
        public ActionResult Index(ProfileViewModel profile)
        {
            return View(new IndexViewModel {
                Budgets = _budgetServices.All().OrderByDescending(x => x.Start).ToList()
            });
        }

        //
        // GET: /Budget/Details/5
        [AuthenticateAttribute]
        public ActionResult Details(ProfileViewModel profile, int id)
        {
            var dto = _budgetServices.Get(id);
            var model = dto.MapTo<DetailsViewModel>();
            model.Categories = dto.Category_Budgets.ToList();

            return View(model);
        }

        //
        // GET: /Budget/Create
        [Authenticate]
        public ActionResult Create(ProfileViewModel profile, int? id)
        {
            var categories = _categoryServices.All().Where(x => x.Type == CategoryTypeDTO.Expense);

            var budgets = new List<SelectListItem>().AsQueryable();

            if (id.HasValue)
            {
                categories = categories.Where(x => x.Account_AccountID == id.Value);

                budgets = _budgetServices.All().Where(x => x.Account_AccountID == id.Value).OrderByDescending(x => x.End).Select(x => new SelectListItem {
                    Value = x.BudgetID.ToString(),
                    Text = x.Start.ToString("dd/MM/yyyy") + " - " + x.End.ToString("dd/MM/yyyy")
                }).AsQueryable();
            }

            var now = DateTime.Now;
            
            var categoryItems = categories.ToList().Select(x => new Category_BudgetViewModel { 
                Category_CategoryID = x.CategoryID,
                Name = x.Name
            }).ToList();

            if (categoryItems == null || categoryItems.Count == 0)
                categoryItems.Add(new Category_BudgetViewModel { Name = "NEW CATEGORY" });

            return View(new CreateViewModel {
                Start = new DateTime(now.Year, now.Month, 1),
                End = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)),
                Account_AccountID = (id.HasValue) ? id.Value : 0,
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices),
                Categories = categoryItems,
                Budgets = budgets,
                RedirectAfterSubmitUrl = _url.Action("Index")
            });
        }

        //
        // GET: /Budget/Create
        [Authenticate]
        [HttpPost]
        public ActionResult CreateFromExistingBudget(ProfileViewModel profile, CreateFromExistingBudgetViewModel model)
        {
            if(model.Budget_BudgetID == 0)
                return RedirectToAction("Create", new { id = model.Account_AccountID });

            var categories = _categoryServices.All().Where(x => x.Type == CategoryTypeDTO.Expense && x.Account_AccountID == model.Account_AccountID);

            var budgets = _budgetServices.All().Where(x => x.Account_AccountID == model.Account_AccountID).OrderByDescending(x => x.End);

            var now = DateTime.Now;

            var categoryAmounts = categories.ToList().Select(x => new Category_BudgetViewModel {
                Category_CategoryID = x.CategoryID,
                Name = x.Name
            }).ToList();

            if(model.Budget_BudgetID != 0) {

                var budget = _budgetServices.Get(model.Budget_BudgetID);

                categoryAmounts = budget.Category_Budgets.Where(x => x.Category_CategoryID != 0).Select(x => new Category_BudgetViewModel { 
                    Name = x.CategoryName,
                    Budget_BudgetID = x.Budget_BudgetID,
                    Category_CategoryID = x.Category_CategoryID,
                    Amount = x.Amount,
                    Total = x.Total
                }).ToList();

                var newCategories = categories.Where(x => !categoryAmounts.Any(c => c.Category_CategoryID == x.CategoryID))
                                              .Select(x => new Category_BudgetViewModel {
                                                  Category_CategoryID = x.CategoryID,
                                                  Name = x.Name
                                              }).ToList();

                categoryAmounts.AddRange(newCategories);
            }

            return View(new CreateFromExistingBudgetViewModel
            {
                Start = new DateTime(now.Year, now.Month, 1),
                End = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)),
                Account_AccountID = model.Account_AccountID,
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices),
                Budget_BudgetID = model.Budget_BudgetID,
                Budgets = budgets.Select(x => new SelectListItem {
                    Value = x.BudgetID.ToString(),
                    Text = x.Start.ToString("dd/MM/yyyy") + " - " + x.End.ToString("dd/MM/yyyy")
                }).AsQueryable(),
                Categories = categoryAmounts,
                RedirectAfterSubmitUrl = _url.Action("Index")
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
                model.Budgets = _budgetServices.All().Where(x => x.Account_AccountID == model.Account_AccountID).Select(x => new SelectListItem {
                    Value = x.BudgetID.ToString(),
                    Text = x.Start.ToString("dd/MM/yyyy") + " - " + x.End.ToString("dd/MM/yyyy")
                }).AsQueryable();
                return View(model);
            }

            var dto = model.MapTo<BudgetDTO>();
            _budgetServices.Save(dto);

            foreach (var category in model.Categories)
            {
                if(category.Delete)
                {
                    _categoryServices.Delete(category.Category_CategoryID);
                }
                else
                {
                    _budgetServices.SaveCategoryBudget(new Category_BudgetDTO { 
                        Account_AccountID = model.Account_AccountID,
                        Budget_BudgetID = dto.BudgetID,
                        Category_CategoryID = category.Category_CategoryID,
                        CategoryName = category.Name,
                        Amount = category.Amount
                    });
                }
            }

            // Clear the cache of anything that depends upon budgets
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Budget));

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        //
        // GET: /Budget/Edit/5
        [Authenticate]
        public ActionResult Edit(ProfileViewModel profile, int id)
        {
            var budget = _budgetServices.Get(id);

            var model = budget.MapTo<EditViewModel>();
            model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
            model.Categories = budget.Category_Budgets.Where(x => x.Category_CategoryID != 0).Select(x => new Category_BudgetViewModel { 
                Name = x.CategoryName,
                Budget_BudgetID = x.Budget_BudgetID,
                Category_CategoryID = x.Category_CategoryID,
                Amount = x.Amount,
                Total = x.Total
            }).ToList();
            model.RedirectAfterSubmitUrl = _url.Action("Index");

            var categories = _categoryServices.All()
                                              .Where(x => x.Account_AccountID == budget.Account_AccountID && x.Type == CategoryTypeDTO.Expense && !model.Categories.Any(c => c.Category_CategoryID == x.CategoryID))
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
                if (category.Delete)
                {
                    // TODO: Should this not be deleting the *join*, rather than the actual category?
                    _categoryServices.Delete(category.Category_CategoryID);
                }
                else
                {
                    _budgetServices.SaveCategoryBudget(new Category_BudgetDTO {
                        Budget_BudgetID = dto.BudgetID,
                        Account_AccountID = model.Account_AccountID,
                        Category_CategoryID = category.Category_CategoryID,
                        CategoryName = category.Name,
                        Amount = category.Amount
                    });
                }
            }

            // Clear the cache of anything that depends upon budgets
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Budget));

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        //
        // POST: /Budget/Delete/5
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(ProfileViewModel profile, int id, string redirectAfterSubmitUrl)
        {
            _budgetServices.Delete(id);

            // Clear the cache of anything that depends upon budgets
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Budget));

            return Redirect(redirectAfterSubmitUrl);
        }
    }
}
