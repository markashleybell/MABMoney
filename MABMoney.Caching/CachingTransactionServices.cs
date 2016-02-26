using MABMoney.Services;
using MABMoney.Services.DTO;
using System;
using System.Collections.Generic;

namespace MABMoney.Caching
{
    public class CachingTransactionServices : CachingServicesBase, ITransactionServices
    {
        ITransactionServices _transactionServices;

        public CachingTransactionServices(ITransactionServices nonCachingTransactionServices, IModelCache cache, ICachingHelpers helpers)
            : base(cache, helpers)
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

        public IEnumerable<TransactionDTO> GetForAccount(int accountId)
        {
            return CacheAndGetValue<IEnumerable<TransactionDTO>>(
                "transactionsforaccount-" + accountId,
                CacheExpiry.OneHour,
                () => _transactionServices.GetForAccount(accountId),
                "transaction", "all"
            );
        }

        public IEnumerable<TransactionDTO> GetForAccount(int accountId, DateTime from, DateTime to)
        {
            return CacheAndGetValue<IEnumerable<TransactionDTO>>(
                "transactionsforaccount-" + accountId + "-" + from.ToString("yyyyMMdd") + "-" + to.ToString("yyyyMMdd"),
                CacheExpiry.OneHour,
                () => _transactionServices.GetForAccount(accountId, from, to),
                "transaction", "all"
            );
        }

        public IEnumerable<TransactionDTO> GetForAccount(int accountId, int categoryId, DateTime from, DateTime to)
        {
            return CacheAndGetValue<IEnumerable<TransactionDTO>>(
                "transactionsforaccount-" + accountId + "-" + categoryId + "-" + from.ToString("yyyyMMdd") + "-" + to.ToString("yyyyMMdd"),
                CacheExpiry.OneHour,
                () => _transactionServices.GetForAccount(accountId, categoryId, from, to),
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
    }
}
