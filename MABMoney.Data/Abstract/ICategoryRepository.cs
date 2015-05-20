using MABMoney.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Data.Abstract
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> All();
        Category Get(int id);
        void Add(Category category);
        void Update(Category category);
        void Delete(int id);
    }
}
