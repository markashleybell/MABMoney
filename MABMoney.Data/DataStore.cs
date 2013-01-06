using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MABMoney.Domain;

namespace MABMoney.Data
{
    public class DataStore : DbContext, IDataStore
    {
        public virtual IDbSet<User> Users { get; set; }
        public virtual IDbSet<Account> Accounts { get; set; }
        public virtual IDbSet<Category> Categories { get; set; }
        public virtual IDbSet<Transaction> Transactions { get; set; }
        public virtual IDbSet<Budget> Budgets { get; set; }
        public virtual IDbSet<Category_Budget> Categories_Budgets { get; set; }
        
        public virtual IDbSet<T> DbSet<T>() where T : class
        {
            return Set<T>();
        }

        public virtual void Commit()
        {
            base.SaveChanges();
        }
    }
}
