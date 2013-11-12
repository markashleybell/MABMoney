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
        TransactionDTO Get(int id);
        void Save(TransactionDTO dto);
        void Delete(int id);

        IEnumerable<TransactionDTO> GetForAccount(int accountId);
    }
}
