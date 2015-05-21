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
            throw new NotImplementedException();
        }

        public void Add(Account account)
        {
            throw new NotImplementedException();
        }

        public void Update(Account account)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
