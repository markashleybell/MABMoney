using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;
using MABMoney.Data;
using MABMoney.Domain;
using mab.lib.SimpleMapper;
using MABMoney.Data.Abstract;

namespace MABMoney.Services
{
    public class TransactionServices : ITransactionServices
    {
        private ITransactionRepository _transactions;
        private IAccountRepository _accounts;

        private int _userId;

        public TransactionServices(ITransactionRepository transactions, IAccountRepository accounts)
        {
            _transactions = transactions;
            _accounts = accounts;
        }

        public IEnumerable<TransactionDTO> All()
        {
            return _transactions.All().MapToList<TransactionDTO>();
        }

        public IEnumerable<TransactionDTO> GetForAccount(int accountId)
        {
            return _transactions.GetForAccount(accountId).MapToList<TransactionDTO>();
        }

        public IEnumerable<TransactionDTO> GetForAccount(int accountId, DateTime from, DateTime to)
        {
            return _transactions.GetForAccount(accountId, from, to).MapToList<TransactionDTO>();
        }

        public IEnumerable<TransactionDTO> GetForAccount(int accountId, int categoryId, DateTime from, DateTime to)
        {
            return _transactions.GetForAccount(accountId, categoryId, from, to).MapToList<TransactionDTO>();
        }

        public TransactionDTO Get(int id)
        {
            return _transactions.Get(id).MapTo<TransactionDTO>();
        }

        public void Save(TransactionDTO dto)
        {
            var account = _accounts.Get(dto.Account_AccountID);

            var transaction = _transactions.Get(dto.TransactionID);

            if (transaction != null)
            {
                dto.MapTo(transaction);
                _transactions.Update(transaction);
            }
            else
            {
                transaction = _transactions.Add(dto.MapTo<Transaction>());
                dto.TransactionID = transaction.TransactionID;
            }

            // Update the transaction description history
            var history = account.TransactionDescriptionHistory;
            var historyList = new List<string>();
            // Turn the history string into a list
            if (!string.IsNullOrWhiteSpace(history))
                historyList = history.Split('|').ToList();
            // Add the description from the transaction we just added to the history
            historyList.Add(dto.Description);
            // Re-flatten the history list (removing any duplicates) and save it against the account
            account.TransactionDescriptionHistory = string.Join("|", historyList.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray());
            _accounts.Update(account);
        }

        public void Delete(int id)
        {
            _transactions.Delete(id);
        }
    }
}
