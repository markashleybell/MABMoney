using MABMoney.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MABMoney.Services.DTO;

namespace MABMoney.Caching
{
    public class CachingAccountServices : CachingServicesBase, IAccountServices
    {
        private IAccountServices _accountServices;

        public CachingAccountServices(IAccountServices nonCachingAccountServices, IModelCache cache, IModelCacheConfiguration cacheConfig) 
            : base(cache, cacheConfig)
        {
            _accountServices = nonCachingAccountServices;
        }

        public IEnumerable<AccountDTO> All()
        {
            return _accountServices.All();
        }

        public AccountDTO Get(int id)
        {
            return _accountServices.Get(id);
        }

        public void Save(AccountDTO dto)
        {
            _accountServices.Save(dto);
        }

        public void Delete(int id)
        {
            _accountServices.Delete(id);
        }
    }
}
