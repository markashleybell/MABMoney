using mab.lib.SimpleMapper;
using MABMoney.Domain;
using MABMoney.Domain.Abstract;
using MABMoney.Services.DTO;
using System;
using System.Collections.Generic;

namespace MABMoney.Services
{
    public class SessionServices : ISessionServices
    {
        private ISessionRepository _sessions;

        public SessionServices(ISessionRepository sessions)
        {
            _sessions = sessions;
        }

        public IEnumerable<SessionDTO> All()
        {
            return _sessions.All().MapToList<SessionDTO>();
        }

        public SessionDTO Get(int id)
        {
            return _sessions.Get(id).MapTo<SessionDTO>();
        }

        public SessionDTO GetByKey(string key)
        {
            return _sessions.GetByKey(key).MapTo<SessionDTO>();
        }

        public void Save(SessionDTO dto)
        {
            // Add the session (sessions will never be updated using this method)
            var session = _sessions.Add(dto.MapTo<Session>());
            dto.SessionID = session.SessionID;
        }

        public void Delete(int id)
        {
            _sessions.Delete(id);
        }

        public void DeleteByKey(string key)
        {
            var session = _sessions.GetByKey(key);

            if (session != null)
            {
                _sessions.Delete(session.SessionID);
            }
        }

        public void DeleteExpired()
        {
            _sessions.DeleteExpired();
        }

        public void UpdateSessionExpiry(string key, DateTime expiry)
        {
            _sessions.UpdateSessionExpiry(key, expiry);
        }
    }
}
