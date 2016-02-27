using MABMoney.Domain;
using MABMoney.Domain.Abstract;
using System.Collections.Generic;

namespace MABMoney.Data.Concrete
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(string connectionString, int userId) : base(connectionString, userId) { }

        public IEnumerable<Account> All()
        {
            return GetEnumerable<Account>("mm_Accounts_Read", new { UserID = _userId });
        }

        public Account Get(int id)
        {
            return GetSingle<Account>("mm_Accounts_Read", new { UserID = _userId, AccountID = id });
        }

        public Account Add(Account account)
        {
            return AddOrUpdate<Account>("mm_Accounts_Create", new {
                UserID = _userId,
                GUID = account.GUID,
                Name = account.Name,
                StartingBalance = account.StartingBalance,
                Default = account.Default,
                Type = account.Type,
                DisplayOrder = account.DisplayOrder,
                IncludeInNetWorth = account.IncludeInNetWorth
            });
        }

        public Account Update(Account account)
        {
            return AddOrUpdate<Account>("mm_Accounts_Update", new {
                UserID = _userId,
                AccountID = account.AccountID,
                Name = account.Name,
                StartingBalance = account.StartingBalance,
                Default = account.Default,
                Type = account.Type,
                DisplayOrder = account.DisplayOrder,
                IncludeInNetWorth = account.IncludeInNetWorth,
                TransactionDescriptionHistory = account.TransactionDescriptionHistory
            });
        }

        public Account Delete(int id)
        {
            return AddOrUpdate<Account>("mm_Accounts_Delete", new { UserID = _userId, AccountID = id });
        }
    }
}
