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
                        Note = t.Note,
                        Category_CategoryID = t.Category_CategoryID,
                        Category = new CategoryDTO { 
                            Name = (t.Category != null) ? t.Category.Name : null
                        },
                        TransferGUID = t.TransferGUID
                    }).ToList();

            // return _transactions.Query(x => x.Account.User_UserID == userId).ToList().MapToList<TransactionDTO>();
        }

        public IEnumerable<TransactionDTO> GetForAccount(int accountId)
        {
            return _transactions.Query(x => x.Account.User_UserID == _userId && x.Account_AccountID == accountId)
                                .Select(t => new TransactionDTO
                                {
                                    TransactionID = t.TransactionID,
                                    Account_AccountID = t.Account_AccountID,
                                    Date = t.Date,
                                    Amount = t.Amount,
                                    Description = t.Description,
                                    Note = t.Note,
                                    Category_CategoryID = t.Category_CategoryID,
                                    Category = new CategoryDTO
                                    {
                                        Name = (t.Category != null) ? t.Category.Name : null
                                    },
                                    TransferGUID = t.TransferGUID
                                }).ToList();
        }

        public IEnumerable<TransactionDTO> GetForAccount(int accountId, DateTime from, DateTime to)
        {
            return _transactions.Query(x => x.Account.User_UserID == _userId && x.Account_AccountID == accountId && x.Date >= from && x.Date <= to)
                                .Select(t => new TransactionDTO
                                {
                                    TransactionID = t.TransactionID,
                                    Account_AccountID = t.Account_AccountID,
                                    Date = t.Date,
                                    Amount = t.Amount,
                                    Description = t.Description,
                                    Note = t.Note,
                                    Category_CategoryID = t.Category_CategoryID,
                                    Category = new CategoryDTO
                                    {
                                        Name = (t.Category != null) ? t.Category.Name : null
                                    },
                                    TransferGUID = t.TransferGUID
                                }).ToList();
        }

        public TransactionDTO Get(int id)
        {
            return _transactions.Query(x => x.Account.User_UserID == _userId && x.TransactionID == id).FirstOrDefault().MapTo<TransactionDTO>();
        }

        public void Save(TransactionDTO dto)
        {
            // Check the account for this transaction exists
            var account = _accounts.Query(x => x.User_UserID == _userId && x.AccountID == dto.Account_AccountID).FirstOrDefault();

            if (account != null)
            {
                // Try and get the transaction
                var transaction = _transactions.Get(dto.TransactionID);

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
            }
        }
    }
}
