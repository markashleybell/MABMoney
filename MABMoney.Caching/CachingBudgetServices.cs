using MABMoney.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MABMoney.Services.DTO;

namespace MABMoney.Caching
{
    public class CachingBudgetServices : IBudgetServices
    {
        private IBudgetServices _budgetServices;
        private IModelCache _cache;
        private IModelCacheConfiguration _cacheConfig;

        public CachingBudgetServices(IBudgetServices nonCachingBudgetServices, IModelCache cache, IModelCacheConfiguration cacheConfig)
        {
            _budgetServices = nonCachingBudgetServices;
            _cache = cache;
            _cacheConfig = cacheConfig;
        }

        public IEnumerable<BudgetDTO> All()
        {
            throw new NotImplementedException();
        }

        public BudgetDTO Get(int id)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void SaveCategoryBudget(Category_BudgetDTO dto)
        {
            throw new NotImplementedException();
        }

        public void DeleteCategoryBudget(int id)
        {
            throw new NotImplementedException();
        }

        public int GetBudgetCount(int accountId)
        {
            throw new NotImplementedException();
        }
    }
}
