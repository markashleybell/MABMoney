using MABMoney.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MABMoney.Services.DTO;

namespace MABMoney.Caching
{
    public class CachingBudgetServices : CachingServicesBase, IBudgetServices
    {
        private IBudgetServices _budgetServices;

        public CachingBudgetServices(IBudgetServices nonCachingBudgetServices, IModelCache cache, IModelCacheConfiguration cacheConfig) 
            : base(cache, cacheConfig)
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

        public BudgetDTO GetLatest(int accountId)
        {
            var key = _cacheConfig.Get<string>("CookieKey") + "-latest-budget-" + accountId;
            var budget = _cache.Get<BudgetDTO>(key);

            if (budget == null)
            {
                budget = _budgetServices.GetLatest(accountId);
                _cache.Add(key, budget, (int)CacheExpiry.OneHour);
            }

            return budget;
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

        public void DeleteCategoryBudget(int id)
        {
            _budgetServices.DeleteCategoryBudget(id);
        }

        public int GetBudgetCount(int accountId)
        {
            return _budgetServices.GetBudgetCount(accountId);
        }
    }
}
