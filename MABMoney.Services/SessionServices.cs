using MABMoney.Data;
using MABMoney.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mab.lib.SimpleMapper;
using MABMoney.Data.Abstract;

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
            throw new NotImplementedException();
        }

        public SessionDTO Get(int id)
        {
            throw new NotImplementedException();
        }

        public SessionDTO GetByUserAndKey(int userId, string key)
        {
            var session = _sessions.Query(x => x.User_UserID == userId && x.Key == key && x.Expiry > _dateProvider.Now)
                                   .Select(x => new SessionDTO { 
                                       SessionID = x.SessionID,
                                       Key = x.Key,
                                       Expiry = x.Expiry,
                                       User_UserID = x.User_UserID
                                   })
                                   .FirstOrDefault();

            if(session == null)
                return null;

            return session.MapTo<SessionDTO>();
        }

        public void Save(SessionDTO dto)
        {
            // Add the session (sessions will never be updated using this method)
            var session = dto.MapTo<Session>();
            _sessions.Add(session);
            _unitOfWork.Commit();

            // Update the DTO with the new ID
            dto.SessionID = session.SessionID;
        }

        public void Delete(int id)
        {
            _sessions.Remove(id);
            _unitOfWork.Commit();
        }

        public void DeleteByKey(string key)
        {
            var session = _sessions.Query(x => x.Key == key).FirstOrDefault();

            if (session != null)
            {
                _sessions.Remove(session.SessionID);
                _unitOfWork.Commit();
            }
        }

        public void DeleteExpiredByUser(int userId)
        {
            foreach (var session in _sessions.Query(x => x.User_UserID == userId && x.Expiry <= _dateProvider.Now).ToList())
                _sessions.Remove(session.SessionID);

            _unitOfWork.Commit();
        }

        public void UpdateSessionExpiry(int userId, string key, DateTime expiry)
        {
            // Try and get the existing session
            var session = _sessions.Query(x => x.User_UserID == userId && x.Key == key && x.Expiry > _dateProvider.Now).FirstOrDefault();

            if (session != null)
            {
                session.Expiry = expiry;
                _unitOfWork.Commit();
            }
        }
    }
}
