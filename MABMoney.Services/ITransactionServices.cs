using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;

namespace MABMoney.Services
{
    public interface ITransactionServices
    {
        IEnumerable<TransactionDTO> All();
        IEnumerable<TransactionDTO> GetForAccount(int accountId);
        IEnumerable<TransactionDTO> GetForAccount(int accountId, DateTime from, DateTime to);
        TransactionDTO Get(int id);
        void Save(TransactionDTO dto);
        void Delete(int id);
    }
}
