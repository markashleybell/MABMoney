using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;

namespace MABMoney.Services
{
    public class TransactionServices : ITransactionServices
    {
        public IEnumerable<TransactionDTO> All()
        {
            throw new NotImplementedException();
        }

        public TransactionDTO Get(int id)
        {
            throw new NotImplementedException();
        }

        public void Save(TransactionDTO dto)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
