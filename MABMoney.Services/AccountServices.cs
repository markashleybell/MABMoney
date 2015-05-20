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
    public class AccountServices : IAccountServices
    {
        private IAccountRepository _accounts;
        private ITransactionRepository _transactions;
        private IUserRepository _users;

        public AccountServices(IAccountRepository accounts, ITransactionRepository transactions, IUserRepository users)
        {
            _accounts = accounts;
            _transactions = transactions;
            _users = users;
        }

        public IEnumerable<AccountDTO> All()
        {
            var dto = _accounts.All().OrderBy(x => x.DisplayOrder).Where(x => x.User_UserID == _userId).Select(a => new AccountDTO { 
                AccountID = a.AccountID,
                Type = (AccountTypeDTO)a.Type,
                Name = a.Name,
                StartingBalance = a.StartingBalance,
                CurrentBalance = a.CurrentBalance,
                Default = a.Default
            }).ToList();

            return dto;
        }

        public IEnumerable<AccountDTO> GetForUser(int userId)
        {
            var dto = _accounts.Query(x => x.User_UserID == _userId).OrderBy(x => x.DisplayOrder).Select(a => new AccountDTO {
                AccountID = a.AccountID,
                Type = (AccountTypeDTO)a.Type,
                Name = a.Name,
                StartingBalance = a.StartingBalance,
                CurrentBalance = a.CurrentBalance,
                Default = a.Default,
                Categories = a.Categories.Select(c => new CategoryDTO { 
                    CategoryID = c.CategoryID,
                    Name = c.Name,
                    Type = (CategoryTypeDTO)c.Type
                }).ToList()
            }).ToList();

            return dto;
        }

        public AccountDTO Get(int id)
        {
            var account = _accounts.Query(x => x.User_UserID == _userId && x.AccountID == id)
                                   .Select(x => new AccountDTO { 
                                       AccountID = x.AccountID,
                                       Name = x.Name,
                                       StartingBalance = x.StartingBalance,
                                       CurrentBalance = x.CurrentBalance,
                                       Default = x.Default,
                                       Type = (AccountTypeDTO)x.Type,
                                       // Project the string value from the database at first
                                       TransactionDescriptionHistoryAsString = x.TransactionDescriptionHistory,
                                       DisplayOrder = x.DisplayOrder
                                   })
                                   .FirstOrDefault();

            if (account == null)
                return null;

            account.TransactionDescriptionHistory = new List<string>();

            // If there's a description history, split the string field from our projected DTO into a list
            if (!string.IsNullOrWhiteSpace(account.TransactionDescriptionHistoryAsString))
                account.TransactionDescriptionHistory = account.TransactionDescriptionHistoryAsString.Split('|').ToList();

            return account;
        }

        public void Save(AccountDTO dto)
        {
            // Try and get the account
            var account = _accounts.Get(dto.AccountID);

            // If the account exists AND belongs to this user
            if (account != null && account.User_UserID == _userId)
            {
                // Update the account
                dto.MapTo(account);

                if (dto.TransactionDescriptionHistory != null)
                {
                    // Convert the list of descriptions back into a flat string, removing duplicates
                    account.TransactionDescriptionHistory = string.Join("|", dto.TransactionDescriptionHistory.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray());
                }

                _unitOfWork.Commit();
            }
            else
            {
                // Add the account
                account = dto.MapTo<Account>();
                account.User_UserID = _userId;
                _accounts.Add(account);
                _unitOfWork.Commit();

                // Update the DTO with the new ID
                dto.AccountID = account.AccountID;
            }

            _unitOfWork.Commit();
        }

        public void Delete(int id)
        {
            var account = _accounts.Query(x => x.User_UserID == _userId && x.AccountID == id).FirstOrDefault();

            if(account != null)
            {
                _accounts.Remove(id);
                _unitOfWork.Commit();
            }
        }
    }
}
