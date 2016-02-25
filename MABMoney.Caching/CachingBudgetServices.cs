using MABMoney.Services;
using MABMoney.Services.DTO;
using System;
using System.Collections.Generic;

namespace MABMoney.Caching
{
    public class CachingBudgetServices : CachingServicesBase, IBudgetServices
    {
        private IBudgetServices _budgetServices;

        public CachingBudgetServices(IBudgetServices nonCachingBudgetServices, IModelCache cache, ICachingHelpers helpers)
            : base(cache, helpers)
        {
            _budgetServices = nonCachingBudgetServices;
        }

        public IEnumerable<BudgetDTO> All()
        {
            return _budgetServices.All();
        }

        public BudgetDTO Get(int id)
        {
            return _budgetServices.Get(id);
        }

        public BudgetDTO GetLatest(DateTime now, int accountId)
        {
            return CacheAndGetValue<BudgetDTO>(
                "latest-budget-" + accountId,
                CacheExpiry.OneHour, 
                () => _budgetServices.GetLatest(now, accountId),
                "budget", "transaction", "category", "all"
            );
        }

        public void Save(BudgetDTO dto)
        {
            _budgetServices.Save(dto);
        }

        public void Delete(int id)
        {
            _budgetServices.Delete(id);
        }

        public void SaveCategoryBudget(Category_BudgetDTO dto)
        {
            _budgetServices.SaveCategoryBudget(dto);
        }

        public int GetBudgetCount(int accountId)
        {
            return CacheAndGetValue<int>(
                "budget-count-" + accountId,
                CacheExpiry.OneHour,
                () => _budgetServices.GetBudgetCount(accountId),
                "budget", "transaction", "all"
            );
        }
    }
}
