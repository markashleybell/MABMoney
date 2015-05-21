using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MABMoney.Data.Abstract;
using MABMoney.Domain;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using Dapper;
using System.Data;
using System.Linq;
using mab.lib.SimpleMapper;

namespace MABMoney.Data.Concrete
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(string connectionString, int userId) : base(connectionString, userId) { }

        public User Get()
        {
            return GetSingle<User>("mm_Users_Read", new { UserID = _userId });
        }

        public void Add(User user)
        {
            var data = new {
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                Password = user.Password,
                IsAdmin = user.IsAdmin
            };
            var item = AddOrUpdate<User>("mm_Users_Create", data);
            item.MapTo(user);
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public User GetByEmailAddress(string email)
        {
            return GetSingle<User>("mm_Users_GetByEmailAddress", new { Email = email });
        }

        public User GetByPasswordResetGUID(string guid)
        {
            return GetSingle<User>("mm_Users_GetByPasswordResetGUID", new { GUID = guid });
        }
    }
}
