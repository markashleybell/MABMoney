using MABMoney.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Data.Abstract
{
    public interface IBudgetRepository
    {
        IEnumerable<Budget> All();
        Budget Get(int id);
        Budget Add(Budget budget);
        Budget Update(Budget budget);
        Budget Delete(int id);

        Budget GetLatest(int accountId, DateTime date);
        int GetBudgetCount(int accountId);
    }
}
