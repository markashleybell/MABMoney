using MABMoney.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Data.Abstract
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> All();
        Transaction Get(int id);
        Transaction Add(Transaction transaction);
        Transaction Update(Transaction transaction);
        Transaction Delete(int id);

        IEnumerable<Transaction> GetForAccount(int accountId);
        IEnumerable<Transaction> GetForAccount(int accountId, DateTime from, DateTime to);
        IEnumerable<Transaction> GetForAccount(int accountId, int categoryId, DateTime from, DateTime to);
    }
}
