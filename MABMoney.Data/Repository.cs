using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Linq.Expressions;

namespace MABMoney.Data
{
    public class Repository<Type, KeyType> : IRepository<Type, KeyType> where Type : class
    {
        private readonly IDbSet<Type> _dbset;
        private readonly IUnitOfWork _unitOfWork;

        public Repository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dbset = _unitOfWork.DataStore.DbSet<Type>();
        }

        public virtual IQueryable<Type> All()
        {
            return _dbset;
        }

        public virtual IQueryable<Type> Query(Expression<Func<Type, bool>> filter)
        {
            return _dbset.Where(filter);
        }

        public virtual Type Get(KeyType key)
        {
            return _dbset.Find(key);
        }

        public virtual void Add(Type entity)
        {
            _dbset.Add(entity);
            _unitOfWork.Commit();
        }

        public virtual void Remove(KeyType key)
        {
            _dbset.Remove(_dbset.Find(key));
            _unitOfWork.Commit();
        }
    }
}
