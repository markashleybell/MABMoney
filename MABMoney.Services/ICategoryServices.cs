using MABMoney.Services.DTO;
using System.Collections.Generic;

namespace MABMoney.Services
{
    public interface ICategoryServices
    {
        IEnumerable<CategoryDTO> All();
        CategoryDTO Get(int id);
        void Save(CategoryDTO dto);
        void Delete(int id);
    }
}
