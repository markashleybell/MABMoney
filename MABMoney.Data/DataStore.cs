﻿using System;
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

        private int _userId;

        public DataStore(int userId)
        {
            _userId = userId;
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
            DateTime now = DateTime.Now;

            foreach (var entry in ChangeTracker.Entries<AuditableEntityBase>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = _userId;
                    entry.Entity.CreatedDate = now;
                    entry.Entity.LastModifiedBy = _userId;
                    entry.Entity.LastModifiedDate = now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedBy = _userId;
                    entry.Entity.LastModifiedDate = now;
                }

                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.Deleted = true;
                    entry.Entity.DeletedBy = _userId;
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
