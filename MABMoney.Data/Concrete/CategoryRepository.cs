using MABMoney.Domain;
using MABMoney.Domain.Abstract;
using System.Collections.Generic;

namespace MABMoney.Data.Concrete
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(string connectionString, int userId) : base(connectionString, userId) { }

        public IEnumerable<Category> All()
        {
            return GetEnumerable<Category>("mm_Categories_Read", new { UserID = _userId });
        }

        public IEnumerable<Category> All(int accountId)
        {
            return GetEnumerable<Category>("mm_Categories_Read", new { UserID = _userId, AccountID = accountId });
        }

        public Category Get(int id)
        {
            return GetSingle<Category>("mm_Categories_Read", new { UserID = _userId, CategoryID = id });
        }

        public Category Add(Category category)
        {
            return AddOrUpdate<Category>("mm_Categories_Create", new {
                UserID = _userId,
                GUID = category.GUID,
                Account_AccountID = category.Account_AccountID,
                Name = category.Name,
                Type = category.Type
            });
        }

        public Category Update(Category category)
        {
            return AddOrUpdate<Category>("mm_Categories_Update", new {
                UserID = _userId,
                CategoryID = category.CategoryID,
                Account_AccountID = category.Account_AccountID,
                Name = category.Name,
                Type = category.Type
            });
        }

        public Category Delete(int id)
        {
            return AddOrUpdate<Category>("mm_Categories_Delete", new { UserID = _userId, CategoryID = id });
        }
    }
}
