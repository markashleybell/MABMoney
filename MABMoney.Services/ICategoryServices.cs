using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;

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
