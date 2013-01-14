using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;

namespace MABMoney.Services
{
    public interface IAccountServices
    {
        IEnumerable<AccountDTO> All(int userId);
        AccountDTO Get(int userId, int id);
        void Save(int userId, AccountDTO dto);
        void Delete(int userId, int id);

        decimal GetNetWorth(int userId);
    }
}
