using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;

namespace MABMoney.Services
{
    public interface IAccountServices
    {
        IEnumerable<AccountDTO> All();
        IEnumerable<AccountDTO> GetForUser(int userId);
        AccountDTO Get(int id);
        void Save(AccountDTO dto);
        void Delete(int id);
    }
}
