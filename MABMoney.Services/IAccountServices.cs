using MABMoney.Services.DTO;
using System.Collections.Generic;

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
