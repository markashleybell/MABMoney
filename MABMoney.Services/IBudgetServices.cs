using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;

namespace MABMoney.Services
{
    public interface IBudgetServices
    {
        IEnumerable<BudgetDTO> All();
        BudgetDTO Get(int id);
        BudgetDTO GetLatest(int accountId);
        void Save(BudgetDTO dto);
        void Delete(int id);

        void SaveCategoryBudget(Category_BudgetDTO dto);
        void DeleteCategoryBudget(int id);
    }
}
