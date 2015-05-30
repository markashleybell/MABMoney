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
            return _accounts.All().MapToList<AccountDTO>();
        }

        public AccountDTO Get(int id)
        {
            var account = _accounts.Get(id).MapTo<AccountDTO>();

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

            // If the account exists
            if (account != null)
            {
                // Update the account
                dto.MapTo(account);

                if (dto.TransactionDescriptionHistory != null)
                {
                    // Convert the list of descriptions back into a flat string, removing duplicates
                    account.TransactionDescriptionHistory = string.Join("|", dto.TransactionDescriptionHistory.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray());
                }
            }
            else
            {
                // Add the account
                account = dto.MapTo<Account>();
                _accounts.Add(account);

                // Update the DTO with the new ID
                dto.AccountID = account.AccountID;
            }
        }

        public void Delete(int id)
        {
            _accounts.Delete(id);
        }
    }
}
