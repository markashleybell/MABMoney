using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MABMoney.Data
{
    public interface IRepository<Type, KeyType> where Type : class
    {
        IQueryable<Type> All();
        IQueryable<Type> Query(Expression<Func<Type, bool>> filter);
        Type Get(KeyType key);
        
        void Add(Type entity);
        void Remove(KeyType key);

        IQueryable<Type> AllDeleted();
        Type GetDeleted(KeyType key);
        IQueryable<Type> QueryDeleted(Expression<Func<Type, bool>> filter);

        //IEnumerable<E> SqlQuery<E>(string sql, params object[] parameters);
        //void ExecuteSqlCommand(string sql, params object[] parameters);
    }
}
