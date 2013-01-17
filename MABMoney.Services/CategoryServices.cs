using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;
using MABMoney.Data;
using MABMoney.Domain;
using mab.lib.SimpleMapper;

namespace MABMoney.Services
{
    public class CategoryServices : ICategoryServices
    {
        private IRepository<Category, int> _categories;
        private IRepository<Account, int> _accounts;
        private IUnitOfWork _unitOfWork;

        public CategoryServices(IRepository<Category, int> categories, IRepository<Account, int> accounts, IUnitOfWork unitOfWork)
        {
            _categories = categories;
            _accounts = accounts;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<CategoryDTO> All(int userId)
        {
            return _categories.Query(x => x.Account.User_UserID == userId).ToList().MapToList<CategoryDTO>();
        }

        public CategoryDTO Get(int userId, int id)
        {
            return _categories.Query(x => x.Account.User_UserID == userId && x.CategoryID == id).FirstOrDefault().MapTo<CategoryDTO>();
        }

        public void Save(int userId, CategoryDTO dto)
        {
            var account = _accounts.Get(dto.Account_AccountID);

            if(account != null && account.User_UserID == userId)
            {
                if (dto.CategoryID == 0)
                {
                    var entity = dto.MapTo<Category>();
                    _categories.Add(entity);
                    _unitOfWork.Commit();
                    dto.CategoryID = entity.CategoryID;
                }
                else
                {
                    var entity = _categories.Get(dto.CategoryID);
                    dto.MapTo(entity);
                    _unitOfWork.Commit();
                }
            }
        }

        public void Delete(int userId, int id)
        {
            var category = _categories.Query(x => x.Account.User_UserID == userId && x.CategoryID == id).FirstOrDefault();

            if(category != null)
            {
                _categories.Remove(id);
                _unitOfWork.Commit();
            }
        }
    }
}
