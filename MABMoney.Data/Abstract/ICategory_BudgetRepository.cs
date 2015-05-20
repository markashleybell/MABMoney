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
        Category_Budget Get(int id);
        void Add(Category_Budget category_budget);
        void Update(Category_Budget category_budget);
        void Delete(int id);
    }
}
