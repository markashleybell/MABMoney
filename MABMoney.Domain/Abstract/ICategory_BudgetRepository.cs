using System.Collections.Generic;

namespace MABMoney.Domain.Abstract
{
    public interface ICategory_BudgetRepository
    {
        IEnumerable<Category_Budget> All(int budgetId);
        IEnumerable<Category_Budget> AllIncludingDeleted(int budgetId);
        Category_Budget Get(int budgetId, int categoryId);
        Category_Budget Add(Category_Budget category_budget);
        Category_Budget Update(Category_Budget category_budget);
        Category_Budget Delete(int budgetId, int categoryId);
    }
}
