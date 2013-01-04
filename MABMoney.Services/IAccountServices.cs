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
        AccountDTO Get(int id);
        void Save(AccountDTO dto);
        void Delete(int id);
    }
}
