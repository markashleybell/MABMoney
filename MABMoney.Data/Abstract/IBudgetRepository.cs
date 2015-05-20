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
        void Add(Budget budget);
        void Update(Budget budget);
        void Delete(int id);

        Budget GetLatest(int accountId);
        int GetBudgetCount(int accountId);
    }
}
