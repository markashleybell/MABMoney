using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MABMoney.Domain;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

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

        public int UserID { get; private set; }

        private IDateTimeProvider _dateProvider;

        public DataStore(int userId, IDateTimeProvider dateProvider)
        {
            UserID = userId;
            _dateProvider = dateProvider;
        }

        public DataStore(int userId, IDateTimeProvider dateProvider, string connectionString) : base(connectionString)
        {
            UserID = userId;
            _dateProvider = dateProvider;
        }

        public virtual IDbSet<T> DbSet<T>() where T : class
        {
            return Set<T>();
        }

        public virtual void Commit()
        {
            SaveChanges();
        }

        public override int SaveChanges()
        {
            DateTime now = _dateProvider.Now;

            foreach (var entry in ChangeTracker.Entries<AuditableEntityBase>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = UserID;
                    entry.Entity.CreatedDate = now;
                    entry.Entity.LastModifiedBy = UserID;
                    entry.Entity.LastModifiedDate = now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedBy = UserID;
                    entry.Entity.LastModifiedDate = now;
                }

                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.Deleted = true;
                    entry.Entity.DeletedBy = UserID;
                    entry.Entity.DeletedDate = now;
                }
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
