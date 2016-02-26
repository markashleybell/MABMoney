using mab.lib.SimpleMapper;
using MABMoney.Domain;
using MABMoney.Domain.Abstract;
using MABMoney.Services.DTO;
using System.Collections.Generic;

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
            return _categories.All().MapToList<CategoryDTO>();
        }

        public CategoryDTO Get(int id)
        {
            return _categories.Get(id).MapTo<CategoryDTO>();
        }

        public void Save(CategoryDTO dto)
        {
            var category = _categories.Get(dto.CategoryID);

            if (category != null)
            {
                dto.MapTo(category);
                _categories.Update(category);
            }
            else
            {
                category = _categories.Add(dto.MapTo<Category>());
                dto.CategoryID = category.CategoryID;
            }
        }

        public void Delete(int id)
        {
            _categories.Delete(id);
        }
    }
}
