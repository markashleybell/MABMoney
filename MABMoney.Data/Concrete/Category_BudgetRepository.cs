using MABMoney.Data.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MABMoney.Domain;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using Dapper;
using System.Data;

namespace MABMoney.Data.Concrete
{
    public class Category_BudgetRepository : BaseRepository, ICategory_BudgetRepository
    {
        public Category_BudgetRepository(string connectionString, int userId) : base(connectionString, userId) { }

        public IEnumerable<Category_Budget> All(int budgetId)
        {
            return GetEnumerable<Category_Budget>("mm_Categories_Budgets_Read", new { 
                UserID = _userId,
                Budget_BudgetID = budgetId 
            });
        }

        public IEnumerable<Category_Budget> AllIncludingDeleted(int budgetId)
        {
            return GetEnumerable<Category_Budget>("mm_Categories_Budgets_Read_Including_Deleted", new {
                UserID = _userId,
                Budget_BudgetID = budgetId
            });
        }

        public Category_Budget Get(int budgetId, int categoryId)
        {
            return GetSingle<Category_Budget>("mm_Categories_Budgets_Read", new { 
                UserID = _userId, 
                Budget_BudgetID = budgetId, 
                Category_CategoryID = categoryId 
            });
        }

        public Category_Budget Add(Category_Budget category_budget)
        {
            return AddOrUpdate<Category_Budget>("mm_Categories_Budgets_Create", new {
                UserID = _userId,
                Budget_BudgetID = category_budget.Budget_BudgetID,
                Category_CategoryID = category_budget.Category_CategoryID,
                Amount = category_budget.Amount
            });
        }

        public Category_Budget Update(Category_Budget category_budget)
        {
            return AddOrUpdate<Category_Budget>("mm_Categories_Budgets_Update", new {
                UserID = _userId,
                Budget_BudgetID = category_budget.Budget_BudgetID,
                Category_CategoryID = category_budget.Category_CategoryID,
                Amount = category_budget.Amount
            });
        }

        public Category_Budget Delete(int budgetId, int categoryId)
        {
            return AddOrUpdate<Category_Budget>("mm_Categories_Budgets_Delete", new { 
                UserID = _userId,
                Budget_BudgetID = budgetId,
                Category_CategoryID = categoryId 
            });
        }
    }
}
