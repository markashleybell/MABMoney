using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;

namespace MABMoney.Services
{
    public interface IUserServices
    {
        IEnumerable<UserDTO> All();
        UserDTO Get(int id);
        UserDTO GetMinimal(int id);
        UserDTO GetByEmailAddress(string email);
        UserDTO GetByPasswordResetGUID(string guid);
        void Save(UserDTO dto);
        void Delete(int id);
    }
}
