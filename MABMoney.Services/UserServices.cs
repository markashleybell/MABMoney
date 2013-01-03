using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Domain;
using MABMoney.Data;

namespace MABMoney.Services
{
    public class UserServices : IUserServices
    {
        private IRepository<User, int> _users;

        public UserServices(IRepository<User, int> users)
        {
            _users = users;
        }

        public User GetUserByID(int id)
        {
            return _users.Get(id);
        }

        public User GetUserByEmailAddress(string email)
        {
            return _users.Query(x => x.Email == email).FirstOrDefault();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _users.All().ToList();
        }
    }
}
