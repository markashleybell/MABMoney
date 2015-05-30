using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;
using MABMoney.Data;
using MABMoney.Domain;
using mab.lib.SimpleMapper;
using MABMoney.Data.Abstract;

namespace MABMoney.Services
{
    public class BudgetServices : IBudgetServices
    {
        private IBudgetRepository _budgets;
        private IAccountRepository _accounts;
        private ICategoryRepository _categories;
        private ICategory_BudgetRepository _categories_budgets;
        private ITransactionRepository _transactions;
        private IAccountServices _accountServices;
        private ICategoryServices _categoryServices;

        public BudgetServices(IBudgetRepository budgets, IAccountRepository accounts, ICategoryRepository categories, ICategory_BudgetRepository categories_budgets, ITransactionRepository transactions, IAccountServices accountServices, ICategoryServices categoryServices)
        {
            _budgets = budgets;
            _accounts = accounts;
            _categories = categories;
            _categories_budgets = categories_budgets;
            _transactions = transactions;
            _accountServices = accountServices;
            _categoryServices = categoryServices;
        }

        public IEnumerable<BudgetDTO> All()
        {
            return _budgets.All().MapToList<BudgetDTO>();
        }

        public BudgetDTO Get(int id)
        {
            return MapBudget(_budgets.Get(id).MapTo<BudgetDTO>());
        }

        public BudgetDTO GetLatest(int accountId)
        {
            var now = DateTime.Now;
            var endOfToday = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            return MapBudget(_budgets.GetLatest(accountId, endOfToday).MapTo<BudgetDTO>());
        }

        private BudgetDTO MapBudget(BudgetDTO budget)
        {
            if (budget == null)
                return null;

            // var balanceAtStart = _transactions.Query(x => x.Account_AccountID == budget.Account_AccountID && x.Date < budget.Start).Sum(x => x.Amount);

            //var balanceAtStart = _transactions.GetForAccount(budget.Account_AccountID, DateTime.MinValue, budget.Start).Sum(x => x.Amount);

            //var budgetTransactions = _transactions.GetForAccount(budget.Account_AccountID, budget.Start, budget.End).Sum(x => x.Amount);

            //// Get any categories which aren't deleted or which were deleted after this budget period ended
            //var category_budgets = _categories_budgets.QueryWithIncludes(x => x.Budget_BudgetID == budget.BudgetID && (!x.Category.Deleted || (x.Category.Deleted && x.Category.DeletedDate > budget.End)), "Category")
            //                                          .Select(x => new Category_BudgetDTO {
            //                                              Budget_BudgetID = x.Budget_BudgetID,
            //                                              Category_CategoryID = x.Category_CategoryID,
            //                                              Category = new CategoryDTO { 
            //                                                  CategoryID = x.Category.CategoryID,
            //                                                  Name = x.Category.Name,
            //                                                  Type = (CategoryTypeDTO)x.Category.Type
            //                                              },
            //                                              Amount = x.Amount
            //                                          })
            //                                          .ToList();

            //if (category_budgets.Count > 0)
            //{
            //    // Assign the categories for this budget
            //    budget.Category_Budgets = category_budgets;
            //    // Work out the total spent in each category so far
            //    budget.Category_Budgets.ForEach(x => x.Total = budgetTransactions.Where(t => t.Category_CategoryID == x.Category_CategoryID).Sum(t => Math.Abs(t.Amount)));

            //    // Work out the total amount overspent across all categories
            //    var overspend = budget.Category_Budgets.Where(x => x.Total > x.Amount).Select(x => x.Total - x.Amount).Sum();

            //    // Work out how much money has been allocated to budget categories
            //    var allocated = budget.Category_Budgets.Sum(x => x.Amount);

            //    // Work out how much money has been spent in budget categories
            //    var account = _accountServices.Get(budget.Account_AccountID);
            //    var allocatedSpent = budget.Category_Budgets.Sum(x => x.Total);

            //    var balanceAtBudgetStart = balanceAtStart + account.StartingBalance;

            //    var unallocatedAmount = ((balanceAtBudgetStart - allocated) - overspend) + budgetTransactions.Where(x => x.Amount > 0).Sum(x => x.Amount);

            //    // Work out how much money was spent in transactions not assigned to a category
            //    var unallocatedSpent = budgetTransactions.Where(x => x.Amount < 0 && x.Category_CategoryID == null).ToList().Sum(x => Math.Abs(x.Amount));

            //    // If there is money left over after all budget category amounts and any overspend have been subtracted
            //    if (unallocatedAmount > 0)
            //    {
            //        // Show how much and how much we've spent so far
            //        budget.Category_Budgets.Add(new Category_BudgetDTO {
            //            Budget_BudgetID = budget.BudgetID,
            //            Category_CategoryID = 0,
            //            Category = new CategoryDTO
            //            {
            //                CategoryID = 0,
            //                Name = "Unallocated",
            //                Type = CategoryTypeDTO.Expense
            //            },
            //            Amount = unallocatedAmount,
            //            Total = unallocatedSpent
            //        });
            //    }
            //}

            return budget;
        }

        public void Save(BudgetDTO dto)
        {
            var budget = _budgets.Get(dto.BudgetID);

            if (budget != null)
            {
                dto.MapTo(budget);
                _budgets.Update(budget);
            }
            else
            {
                budget = _budgets.Add(dto.MapTo<Budget>());
                dto.BudgetID = budget.BudgetID;
            }
        }

        public void Delete(int id)
        {
            _budgets.Delete(id);
        }

        public void SaveCategoryBudget(Category_BudgetDTO dto)
        {
            _categoryServices.Save(dto.Category);

            dto.Category_CategoryID = dto.Category.CategoryID;

            var cb = _categories_budgets.Get(dto.Budget_BudgetID, dto.Category_CategoryID);

            if (cb != null)
            {
                dto.MapTo(cb);
                _categories_budgets.Update(cb);
            }
            else
            {
                cb = _categories_budgets.Add(dto.MapTo<Category_Budget>());
            }
        }

        public void DeleteCategoryBudget(int id)
        {
            throw new NotImplementedException();
        }

        public int GetBudgetCount(int accountId)
        {
            return _budgets.GetBudgetCount(accountId);
        }
    }
}
