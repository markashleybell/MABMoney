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

namespace MABMoney.Data.Concrete
{
    public class CategoryRepository : ICategoryRepository
    {
        public IEnumerable<Category> All()
        {
            throw new NotImplementedException();
        }

        public Category Get(int id)
        {
            throw new NotImplementedException();
        }

        public void Add(Category category)
        {
            throw new NotImplementedException();
        }

        public void Update(Category category)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
