using MABMoney.Data;
using MABMoney.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mab.lib.SimpleMapper;

namespace MABMoney.Services
{
    public class SessionServices : ISessionServices
    {
        private IDateTimeProvider _dateProvider;
        private IRepository<Session, int> _sessions;
        private IUnitOfWork _unitOfWork;

        private int _userId;

        public SessionServices(IDateTimeProvider dateProvider, IRepository<Session, int> sessions, IUnitOfWork unitOfWork)
        {
            _dateProvider = dateProvider;
            _sessions = sessions;
            _unitOfWork = unitOfWork;
            _userId = unitOfWork.DataStore.UserID;
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
            var session = _sessions.Query(x => x.User_UserID == userId && x.Key == key && x.Expiry > _dateProvider.Now).FirstOrDefault();

            if(session == null)
                return null;

            return session.MapTo<SessionDTO>();
        }

        public void Save(SessionDTO dto)
        {
            // Add the session (session records will never be updated)
            var session = dto.MapTo<Session>();
            _sessions.Add(session);
            _unitOfWork.Commit();

            // Update the DTO with the new ID
            dto.SessionID = session.SessionID;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
