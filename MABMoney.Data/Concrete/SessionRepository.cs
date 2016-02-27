using MABMoney.Domain;
using MABMoney.Domain.Abstract;
using System;
using System.Collections.Generic;

namespace MABMoney.Data.Concrete
{
    public class SessionRepository : BaseRepository, ISessionRepository
    {
        public SessionRepository(string connectionString, int userId) : base(connectionString, userId) { }

        public IEnumerable<Session> All()
        {
            return GetEnumerable<Session>("mm_Sessions_Read", new { UserID = _userId });
        }

        public Session Get(int id)
        {
            return GetSingle<Session>("mm_Sessions_Read", new { UserID = _userId, SessionID = id });
        }

        public Session Add(Session session)
        {
            return AddOrUpdate<Session>("mm_Sessions_Create", new {
                UserID = session.User_UserID, // We must allow this to be passed in or login cannot work...
                GUID = session.GUID,
                Key = session.Key,
                Expiry = session.Expiry
            });
        }

        public Session Update(Session session)
        {
            return AddOrUpdate<Session>("mm_Sessions_Update", new {
                UserID = _userId,
                SessionID = session.SessionID,
                Key = session.Key,
                Expiry = session.Expiry
            });
        }

        public Session Delete(int id)
        {
            return AddOrUpdate<Session>("mm_Sessions_Delete", new { UserID = _userId, SessionID = id });
        }

        public Session GetByKey(string key)
        {
            return GetSingle<Session>("mm_Sessions_Read", new { UserID = _userId, Key = key });
        }
        
        public Session UpdateSessionExpiry(string key, DateTime expiry)
        {
            var session = GetByKey(key);

            if (session == null)
                return null;

            return AddOrUpdate<Session>("mm_Sessions_Update", new {
                UserID = _userId,
                SessionID = session.SessionID,
                Key = key,
                Expiry = expiry
            });
        }

        public Session DeleteByKey(string key)
        {
            var session = GetByKey(key);

            if (session == null)
                return null;

            return AddOrUpdate<Session>("mm_Sessions_Delete", new { UserID = _userId, SessionID = session.SessionID });
        }

        public void DeleteExpired()
        {
            Execute("mm_Sessions_Delete_Expired", new { UserID = _userId });
        }
    }
}
