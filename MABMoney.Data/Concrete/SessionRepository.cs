using MABMoney.Data.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MABMoney.Domain;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using Dapper;
using System.Data;

namespace MABMoney.Data.Concrete
{
    public class SessionRepository : ISessionRepository
    {
        public IEnumerable<Session> All()
        {
            throw new NotImplementedException();
        }

        public Session Get(int id)
        {
            throw new NotImplementedException();
        }

        public void Add(Session session)
        {
            throw new NotImplementedException();
        }

        public void Update(Session session)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Session GetByUserAndKey(string key)
        {
            throw new NotImplementedException();
        }

        public void DeleteByKey(string key)
        {
            throw new NotImplementedException();
        }

        public void DeleteExpiredForCurrentUser()
        {
            throw new NotImplementedException();
        }

        public void UpdateSessionExpiryForCurrentUser(string key, DateTime expiry)
        {
            throw new NotImplementedException();
        }
    }
}
