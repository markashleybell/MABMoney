using MABMoney.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MABMoney.Services.DTO;

namespace MABMoney.Caching
{
    public class CachingTransactionServices : CachingServicesBase, ITransactionServices
    {
        ITransactionServices _transactionServices;

        public CachingTransactionServices(ITransactionServices nonCachingTransactionServices, IModelCache cache, IModelCacheConfiguration cacheConfig) 
            : base(cache, cacheConfig)
        {
            _transactionServices = nonCachingTransactionServices;
        }

        public IEnumerable<TransactionDTO> All()
        {
            return CacheAndGetValue<IEnumerable<TransactionDTO>>(
                "alltransactions", 
                CacheExpiry.OneHour,
                () => _transactionServices.All(),
                "transaction", "all"
            );
        }

        public TransactionDTO Get(int id)
        {
            return _transactionServices.Get(id);
        }

        public void Save(TransactionDTO dto)
        {
            _transactionServices.Save(dto);
        }

        public void Delete(int id)
        {
            _transactionServices.Delete(id);
        }

        public IEnumerable<TransactionDTO> GetForAccount(int accountId)
        {
            return _transactionServices.GetForAccount(accountId);
        }
    }
}
