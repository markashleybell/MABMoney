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
        private IUnitOfWork _unitOfWork;

        public CategoryServices(IRepository<Category, int> categories, IUnitOfWork unitOfWork)
        {
            _categories = categories;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<CategoryDTO> All()
        {
            return _categories.All().ToList().MapToList<CategoryDTO>();
        }

        public CategoryDTO Get(int id)
        {
            return _categories.Get(id).MapTo<CategoryDTO>();
        }

        public void Save(CategoryDTO dto)
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

        public void Delete(int id)
        {
            _categories.Remove(id);
            _unitOfWork.Commit();
        }
    }
}
