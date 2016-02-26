using System.Collections.Generic;

namespace MABMoney.Domain.Abstract
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
