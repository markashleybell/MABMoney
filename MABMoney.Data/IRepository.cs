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
    }
}
