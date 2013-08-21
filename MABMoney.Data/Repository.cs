using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Data
{
    public class Repository<Type, KeyType> : IRepository<Type, KeyType> where Type : MABMoney.Domain.AuditableEntityBase
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
            return _dbset.Where(x => !x.Deleted);
        }

        public virtual IQueryable<Type> Query(Expression<Func<Type, bool>> filter)
        {
            return _dbset.Where(x => !x.Deleted).Where(filter);
        }

        public virtual IQueryable<Type> QueryWithIncludes(Expression<Func<Type, bool>> filter, params string[] includes)
        {
            var results = _dbset.Where(x => !x.Deleted).Where(filter);

            foreach (var include in includes)
                results = results.Include(include);

            return results;
        }

        public virtual Type Get(KeyType key)
        {
            var result = _dbset.Find(key);

            if(result == null || result.Deleted)
                return null;

            return result;
        }

        public virtual void Add(Type entity)
        {
            _dbset.Add(entity);
        }

        public virtual void Remove(KeyType key)
        {
            _dbset.Remove(_dbset.Find(key));
        }

        public virtual IQueryable<Type> AllDeleted()
        {
            return _dbset.Where(x => x.Deleted);
        }

        public virtual IQueryable<Type> QueryDeleted(Expression<Func<Type, bool>> filter)
        {
            return _dbset.Where(x => x.Deleted).Where(filter);
        }

        public virtual Type GetDeleted(KeyType key)
        {
            var result = _dbset.Find(key);

            if (result == null || !result.Deleted)
                return null;

            return result;
        }

        //public virtual IEnumerable<E> SqlQuery<E>(string sql, params object[] parameters)
        //{
        //    var paramList = new List<SqlParameter>();

        //    for (var i = 0; i < parameters.Length; i++)
        //    {
        //        paramList.Add(new SqlParameter("@p" + i, parameters[i]));
        //    }

        //    if (paramList.Count > 0)
        //        return _unitOfWork.Database.RawData.SqlQuery<E>(sql, paramList.ToArray());
        //    else
        //        return _unitOfWork.Database.RawData.SqlQuery<E>(sql);
        //}

        //public virtual void ExecuteSqlCommand(string sql, params object[] parameters)
        //{
        //    var paramList = new List<SqlParameter>();

        //    for (var i = 0; i < parameters.Length; i++)
        //    {
        //        if (parameters[i] == null)
        //            paramList.Add(new SqlParameter("@p" + i, DBNull.Value));
        //        else
        //            paramList.Add(new SqlParameter("@p" + i, parameters[i]));
        //    }

        //    if (paramList.Count > 0)
        //        _unitOfWork.Database.RawData.ExecuteSqlCommand(sql, paramList.ToArray());
        //    else
        //        _unitOfWork.Database.RawData.ExecuteSqlCommand(sql);
        //}
    }
}
