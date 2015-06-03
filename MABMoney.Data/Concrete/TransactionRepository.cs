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
            return GetEnumerable<Transaction>("mm_Transactions_Read", new { UserID = _userId });
        }

        public Transaction Get(int id)
        {
            return GetSingle<Transaction>("mm_Transactions_Read", new { UserID = _userId, TransactionID = id });
        }

        public Transaction Add(Transaction transaction)
        {
            return AddOrUpdate<Transaction>("mm_Transactions_Create", new {
                UserID = _userId,
                Account_AccountID = transaction.Account_AccountID,
                Category_CategoryID = transaction.Category_CategoryID,
                Date = transaction.Date,
                Description = transaction.Description,
                Note = transaction.Note,
                Amount = transaction.Amount,
                TransferGUID = transaction.TransferGUID
            });
        }

        public Transaction Update(Transaction transaction)
        {
            return AddOrUpdate<Transaction>("mm_Transactions_Update", new {
                TransactionID = transaction.TransactionID,
                UserID = _userId,
                Account_AccountID = transaction.Account_AccountID,
                Category_CategoryID = transaction.Category_CategoryID,
                Date = transaction.Date,
                Description = transaction.Description,
                Note = transaction.Note,
                Amount = transaction.Amount,
                TransferGUID = transaction.TransferGUID
            });
        }

        public Transaction Delete(int id)
        {
            return AddOrUpdate<Transaction>("mm_Transactions_Delete", new { UserID = _userId, TransactionID = id });
        }

        public IEnumerable<Transaction> GetForAccount(int accountId)
        {
            return GetEnumerable<Transaction>("mm_Transactions_Read", new { UserID = _userId, AccountID = accountId });
        }

        public IEnumerable<Transaction> GetForAccount(int accountId, DateTime from, DateTime to)
        {
            return GetEnumerable<Transaction>("mm_Transactions_Read", new {
                UserID = _userId,
                AccountID = accountId,
                From = from,
                To = to
            });
        }

        public IEnumerable<Transaction> GetForAccount(int accountId, int categoryId, DateTime from, DateTime to)
        {
            return GetEnumerable<Transaction>("mm_Transactions_Read", new { 
                UserID = _userId, 
                AccountID = accountId,
                CategoryID = categoryId,
                From = from,
                To = to
            });
        }

        public decimal GetTotalForAccountUpTo(int accountId, DateTime date)
        {
            return GetScalar<decimal>("mm_Transactions_GetTotalUpTo", new {
                UserID = _userId,
                AccountID = accountId,
                Date = date
            });
        }
    }
}
