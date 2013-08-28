using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;
using MABMoney.Data;
using MABMoney.Domain;
using mab.lib.SimpleMapper;

namespace MABMoney.Services
{
    public class TransactionServices : ITransactionServices
    {
        private IRepository<Transaction, int> _transactions;
        private IRepository<Account, int> _accounts;
        private IUnitOfWork _unitOfWork;

        private int _userId;

        public TransactionServices(IRepository<Transaction, int> transactions, IRepository<Account, int> accounts, IUnitOfWork unitOfWork)
        {
            _transactions = transactions;
            _accounts = accounts;
            _unitOfWork = unitOfWork;
            _userId = unitOfWork.DataStore.UserID;
        }

        public IEnumerable<TransactionDTO> All()
        {
            return (from t in _transactions.Query(x => x.Account.User_UserID == _userId)
                    select new TransactionDTO
                    {
                        TransactionID = t.TransactionID,
                        Account_AccountID = t.Account_AccountID,
                        Account = new AccountDTO {
                            Name = t.Account.Name
                        },
                        Date = t.Date,
                        Amount = t.Amount,
                        Description = t.Description,
                        Category_CategoryID = t.Category_CategoryID,
                        Category = new CategoryDTO { 
                            Name = (t.Category != null) ? t.Category.Name : null
                        },
                        TransferGUID = t.TransferGUID
                    }).ToList();

            // return _transactions.Query(x => x.Account.User_UserID == userId).ToList().MapToList<TransactionDTO>();
        }

        public TransactionDTO Get(int id)
        {
            return _transactions.Query(x => x.Account.User_UserID == _userId && x.TransactionID == id).FirstOrDefault().MapTo<TransactionDTO>();
        }

        public void Save(TransactionDTO dto)
        {
            // Get all the transactions for this user/account 
            var account = _accounts.Query(x => x.User_UserID == _userId && x.AccountID == dto.Account_AccountID).FirstOrDefault();

            if (account != null)
            {
                // Try and get the transaction
                var transaction = account.Transactions.FirstOrDefault(x => x.TransactionID == dto.TransactionID);

                // If the transaction exists AND belongs to this user
                if (transaction != null && transaction.Account.User_UserID == _userId)
                {
                    // Update the transaction
                    dto.MapTo(transaction);
                    _unitOfWork.Commit();
                }
                else
                {
                    // Add the transaction
                    transaction = dto.MapTo<Transaction>();
                    _transactions.Add(transaction);
                    _unitOfWork.Commit();

                    // Update the DTO with the new ID
                    dto.TransactionID = transaction.TransactionID;
                }

                // Update the current balance for this account
                account.CurrentBalance = account.StartingBalance + (account.Transactions.Where(t => !t.Deleted).Select(t => t.Amount).DefaultIfEmpty(0).Sum());
                _unitOfWork.Commit();
            }
        }

        public void Delete(int id)
        {
            var transaction = _transactions.Query(x => x.Account.User_UserID == _userId && x.TransactionID == id).FirstOrDefault();

            if(transaction != null)
            {
                _transactions.Remove(id);
                _unitOfWork.Commit();

                // Update the current balance for this account
                transaction.Account.CurrentBalance = transaction.Account.StartingBalance + (transaction.Account.Transactions.Where(t => !t.Deleted).Select(t => t.Amount).DefaultIfEmpty(0).Sum());
                _unitOfWork.Commit();
            }
        }
    }
}
