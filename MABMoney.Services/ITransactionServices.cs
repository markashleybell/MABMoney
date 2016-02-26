using MABMoney.Services.DTO;
using System;
using System.Collections.Generic;

namespace MABMoney.Services
{
    public interface ITransactionServices
    {
        IEnumerable<TransactionDTO> All();
        IEnumerable<TransactionDTO> GetForAccount(int accountId);
        IEnumerable<TransactionDTO> GetForAccount(int accountId, DateTime from, DateTime to);
        IEnumerable<TransactionDTO> GetForAccount(int accountId, int categoryId, DateTime from, DateTime to);
        TransactionDTO Get(int id);
        void Save(TransactionDTO dto);
        void Delete(int id);
    }
}
