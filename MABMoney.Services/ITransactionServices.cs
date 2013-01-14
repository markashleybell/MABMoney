using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;

namespace MABMoney.Services
{
    public interface ITransactionServices
    {
        IEnumerable<TransactionDTO> All(int userId);
        TransactionDTO Get(int userId, int id);
        void Save(int userId, TransactionDTO dto);
        void Delete(int userId, int id);
    }
}
