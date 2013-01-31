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

        private int _userId;

        public CategoryServices(IRepository<Category, int> categories, IRepository<Account, int> accounts, IUnitOfWork unitOfWork)
        {
            _categories = categories;
            _accounts = accounts;
            _unitOfWork = unitOfWork;
            _userId = unitOfWork.DataStore.UserID;
        }

        public IEnumerable<CategoryDTO> All()
        {
            return (from c in _categories.Query(x => x.Account.User_UserID == _userId)
                    select new CategoryDTO
                    {
                        CategoryID = c.CategoryID,
                        Account_AccountID = c.Account_AccountID,
                        Name = c.Name,
                        Account = new AccountDTO
                        {
                            Name = c.Account.Name
                        },
                        Type = (CategoryTypeDTO)c.Type
                    }).ToList();
        }

        public CategoryDTO Get(int id)
        {
            return _categories.Query(x => x.Account.User_UserID == _userId && x.CategoryID == id).FirstOrDefault().MapTo<CategoryDTO>();
        }

        public void Save(CategoryDTO dto)
        {
            // Try and get the category
            var category = _categories.Get(dto.CategoryID);

            // If the category exists AND belongs to this user
            if (category != null && category.Account.User_UserID == _userId)
            {
                // Update the category
                dto.MapTo(category);
                _unitOfWork.Commit();
            }
            else
            {
                // Add the category
                category = dto.MapTo<Category>();
                _categories.Add(category);
                _unitOfWork.Commit();

                // Update the DTO with the new ID
                dto.CategoryID = category.CategoryID;
            }
        }

        public void Delete(int id)
        {
            var category = _categories.Query(x => x.Account.User_UserID == _userId && x.CategoryID == id).FirstOrDefault();

            if(category != null)
            {
                _categories.Remove(id);
                _unitOfWork.Commit();
            }
        }
    }
}
