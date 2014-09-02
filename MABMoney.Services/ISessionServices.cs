using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Services
{
    public interface ISessionServices
    {
        IEnumerable<SessionDTO> All();
        SessionDTO Get(int id);
        SessionDTO GetByUserAndKey(int userId, string key);
        void Save(SessionDTO dto);
        void Delete(int id);
    }
}
