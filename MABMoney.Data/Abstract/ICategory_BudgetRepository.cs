using MABMoney.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Data.Abstract
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
