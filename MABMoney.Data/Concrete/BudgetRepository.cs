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
    public class BudgetRepository : BaseRepository, IBudgetRepository
    {
        public BudgetRepository(string connectionString, int userId) : base(connectionString, userId) { }

        public IEnumerable<Budget> All()
        {
            throw new NotImplementedException();
        }

        public Budget Get(int id)
        {
            throw new NotImplementedException();
        }

        public void Add(Budget budget)
        {
            throw new NotImplementedException();
        }

        public void Update(Budget budget)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Budget GetLatest(int accountId)
        {
            throw new NotImplementedException();
        }

        public int GetBudgetCount(int accountId)
        {
            throw new NotImplementedException();
        }
    }
}
