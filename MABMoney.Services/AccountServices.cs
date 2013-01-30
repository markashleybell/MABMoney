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
    public class AccountServices : IAccountServices
    {
        private IRepository<Account, int> _accounts;
        private IRepository<Transaction, int> _transactions;
        private IRepository<User, int> _users;
        private IUnitOfWork _unitOfWork;

        public AccountServices(IRepository<Account, int> accounts, IRepository<Transaction, int> transactions, IRepository<User, int> users, IUnitOfWork unitOfWork)
        {
            _accounts = accounts;
            _transactions = transactions;
            _users = users;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<AccountDTO> All(int userId)
        {
            var dto = _accounts.Query(x => x.User_UserID == userId).Select(a => new AccountDTO { 
                AccountID = a.AccountID,
                Type = (AccountTypeDTO)a.Type,
                Name = a.Name,
                StartingBalance = a.StartingBalance,
                // CurrentBalance = a.StartingBalance + (a.Transactions.Where(t => !t.Deleted).Select(t => (decimal?)t.Amount).Sum() ?? 0M),
                CurrentBalance = a.StartingBalance + (a.Transactions.Where(t => !t.Deleted).Select(t => t.Amount).DefaultIfEmpty(0).Sum()),
                Default = a.Default
            }).ToList();

            return dto;
        }

        public AccountDTO Get(int userId, int id)
        {
            var dto = _accounts.Query(x => x.User_UserID == userId && x.AccountID == id).FirstOrDefault().MapTo<AccountDTO>();
            var transactions = _transactions.Query(x => x.Account_AccountID == dto.AccountID).ToList();
            dto.CurrentBalance = dto.StartingBalance + transactions.Sum(x => x.Amount);

            return dto;
        }

        public void Save(int userId, AccountDTO dto)
        {
            var account = _accounts.Query(x => x.User_UserID == userId && x.AccountID == dto.AccountID).FirstOrDefault();

            if (account != null)
            {
                dto.User_UserID = userId;

                if (dto.AccountID == 0)
                {
                    var entity = dto.MapTo<Account>();
                    _accounts.Add(entity);
                    _unitOfWork.Commit(userId);
                    dto.AccountID = entity.AccountID;
                }
                else
                {
                    var entity = _accounts.Get(dto.AccountID);
                    dto.MapTo(entity);
                    _unitOfWork.Commit(userId);
                }
            }
        }

        public void Delete(int userId, int id)
        {
            var account = _accounts.Query(x => x.User_UserID == userId && x.AccountID == id).FirstOrDefault();

            if(account != null)
            {
                _accounts.Remove(id);
                _unitOfWork.Commit(userId);
            }
        }
    }
}
