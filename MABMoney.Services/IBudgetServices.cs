using MABMoney.Services.DTO;
using System;
using System.Collections.Generic;

namespace MABMoney.Services
{
    public interface IBudgetServices
    {
        IEnumerable<BudgetDTO> All();
        BudgetDTO Get(int id);
        BudgetDTO GetLatest(DateTime now, int accountId);
        void Save(BudgetDTO dto);
        void Delete(int id);

        void SaveCategoryBudget(Category_BudgetDTO dto);

        int GetBudgetCount(int accountId);
    }
}
