using System;
using System.Collections.Generic;

namespace MABMoney.Domain.Abstract
{
    public interface ISessionRepository
    {
        IEnumerable<Session> All();
        Session Get(int id);
        Session Add(Session session);
        Session Update(Session session);
        Session Delete(int id);

        Session GetByKey(string key);
        Session UpdateSessionExpiry(string key, DateTime expiry);
        Session DeleteByKey(string key);
        void DeleteExpired();
    }
}
