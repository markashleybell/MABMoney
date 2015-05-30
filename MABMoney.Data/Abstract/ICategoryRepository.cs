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
        IEnumerable<Category> All(int accountId);
        Category Get(int id);
        Category Add(Category category);
        Category Update(Category category);
        Category Delete(int id);
    }
}
