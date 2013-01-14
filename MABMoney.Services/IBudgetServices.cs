using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;

namespace MABMoney.Services
{
    public interface IBudgetServices
    {
        IEnumerable<BudgetDTO> All(int userId);
        BudgetDTO Get(int userId, int id);
        BudgetDTO GetLatest(int userId, int accountId);
        void Save(int userId, BudgetDTO dto);
        void Delete(int userId, int id);

        void SaveCategoryBudget(int userId, Category_BudgetDTO dto);
        void DeleteCategoryBudget(int userId, int id);
    }
}
