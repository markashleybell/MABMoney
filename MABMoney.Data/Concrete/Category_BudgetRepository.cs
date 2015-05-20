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
            throw new NotImplementedException();
        }

        public Category_Budget Get(int id)
        {
            throw new NotImplementedException();
        }

        public void Add(Category_Budget category_budget)
        {
            throw new NotImplementedException();
        }

        public void Update(Category_Budget category_budget)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
