using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MABMoney.Domain;

namespace MABMoney.Data
{
    public interface IDataStore
    {
        IDbSet<User> Users { get; }
        IDbSet<Account> Accounts { get; }
        IDbSet<Category> Categories { get; }
        IDbSet<Transaction> Transactions { get; }

        IDbSet<T> DbSet<T>() where T : class;

        void Commit();
        void Dispose();
    }
}
