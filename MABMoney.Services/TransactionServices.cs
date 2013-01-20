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

        public TransactionServices(IRepository<Transaction, int> transactions, IRepository<Account, int> accounts, IUnitOfWork unitOfWork)
        {
            _transactions = transactions;
            _accounts = accounts;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<TransactionDTO> All(int userId)
        {
            return (from t in _transactions.Query(x => x.Account.User_UserID == userId)
                    select new TransactionDTO
                    {
                        TransactionID = t.TransactionID,
                        Account_AccountID = t.Account_AccountID,
                        Date = t.Date,
                        Amount = t.Amount,
                        Description = t.Description,
                        Category_CategoryID = t.Category_CategoryID,
                        Category = new CategoryDTO { 
                            Name = (t.Category != null) ? t.Category.Name : null
                        }
                    }).ToList();

            // return _transactions.Query(x => x.Account.User_UserID == userId).ToList().MapToList<TransactionDTO>();
        }

        public TransactionDTO Get(int userId, int id)
        {
            return _transactions.Query(x => x.Account.User_UserID == userId && x.TransactionID == id).FirstOrDefault().MapTo<TransactionDTO>();
        }

        public void Save(int userId, TransactionDTO dto)
        {
            var account = _accounts.Get(dto.Account_AccountID);

            if(account != null && account.User_UserID == userId)
            {
                if (dto.TransactionID == 0)
                {
                    var entity = dto.MapTo<Transaction>();
                    _transactions.Add(entity);
                    _unitOfWork.Commit(userId);
                    dto.TransactionID = entity.TransactionID;
                }
                else
                {
                    var entity = _transactions.Get(dto.TransactionID);
                    dto.MapTo(entity);
                    _unitOfWork.Commit(userId);
                }
            }
        }

        public void Delete(int userId, int id)
        {
            var transaction = _transactions.Query(x => x.Account.User_UserID == userId && x.TransactionID == id).FirstOrDefault();

            if(transaction != null)
            {
                _transactions.Remove(id);
                _unitOfWork.Commit(userId);
            }
        }
    }
}
