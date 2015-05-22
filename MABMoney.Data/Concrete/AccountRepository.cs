using MABMoney.Data.Abstract;
using mab.lib.SimpleMapper;
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
using System.Data.SqlClient;

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
                Name = account.Name,
                StartingBalance = account.StartingBalance,
                Default = account.Default,
                Type = account.Type,
                DisplayOrder = account.DisplayOrder
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
                DisplayOrder = account.DisplayOrder
            });
        }

        public Account Delete(int id)
        {
            return AddOrUpdate<Account>("mm_Accounts_Delete", new { 
                UserID = _userId, 
                AccountID = id 
            });
        }
    }
}
