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

        public CachingAccountServices(IAccountServices nonCachingAccountServices, IModelCache cache, ICachingHelpers helpers)
            : base(cache, helpers)
        {
            _accountServices = nonCachingAccountServices;
        }

        public IEnumerable<AccountDTO> All()
        {
            return CacheAndGetValue<IEnumerable<AccountDTO>>(
                "accounts",
                CacheExpiry.OneHour,
                () => _accountServices.All(),
                "account", "transaction", "all"
            );
        }

        public IEnumerable<AccountDTO> GetForUser(int userId)
        {
            return _accountServices.GetForUser(userId);
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
