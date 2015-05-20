using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;
using MABMoney.Data;
using MABMoney.Domain;
using mab.lib.SimpleMapper;
using MABMoney.Data.Abstract;

namespace MABMoney.Services
{
    public class CategoryServices : ICategoryServices
    {
        private ICategoryRepository _categories;
        private IAccountRepository _accounts;

        public CategoryServices(ICategoryRepository categories, IAccountRepository accounts)
        {
            _categories = categories;
            _accounts = accounts;
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
                // Check if a category exactly matching this has previously been added and deleted by this person
                var deleted = _categories.QueryDeleted(x => x.Name == dto.Name && x.Account_AccountID == dto.Account_AccountID && x.Type == (CategoryType)dto.Type)
                                         .FirstOrDefault();

                if (deleted != null)
                {
                    // 'Undelete' the category
                    deleted.Deleted = false;
                    deleted.DeletedBy = null;
                    deleted.DeletedDate = null;
                    _unitOfWork.Commit();

                    // Update the DTO with the new ID
                    dto.CategoryID = deleted.CategoryID;
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
