using MABMoney.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Data.Abstract
{
    public interface ISessionRepository
    {
        IEnumerable<Session> All();
        Session Get(int id);
        void Add(Session session);
        void Update(Session session);
        void Delete(int id);

        Session GetByUserAndKey(string key);
        void DeleteByKey(string key);
        void DeleteExpiredForCurrentUser();
        void UpdateSessionExpiryForCurrentUser(string key, DateTime expiry);
    }
}
