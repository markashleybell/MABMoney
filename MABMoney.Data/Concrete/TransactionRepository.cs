using MABMoney.Data.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MABMoney.Domain;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using Dapper;
using System.Data;

namespace MABMoney.Data.Concrete
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        public TransactionRepository(string connectionString, int userId) : base(connectionString, userId) { }

        public IEnumerable<Transaction> All()
        {
            throw new NotImplementedException();
        }

        public Transaction Get(int id)
        {
            throw new NotImplementedException();
        }

        public void Add(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public void Update(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> GetForAccount(int accountId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> GetForAccount(int accountId, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> GetForAccount(int accountId, int categoryId, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }
    }
}
