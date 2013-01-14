using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;

namespace MABMoney.Services
{
    public interface ICategoryServices
    {
        IEnumerable<CategoryDTO> All(int userId);
        CategoryDTO Get(int userId, int id);
        void Save(int userId, CategoryDTO dto);
        void Delete(int userId, int id);
    }
}
