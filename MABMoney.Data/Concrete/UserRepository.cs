using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MABMoney.Data.Abstract;
using MABMoney.Domain;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using Dapper;
using System.Data;

namespace MABMoney.Data.Concrete
{
    public class UserRepository : IUserRepository
    {
        private string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<User> All()
        {
            throw new NotImplementedException();
        }

        public User Get(int id)
        {
            using(var connection = new ProfiledDbConnection(new SqlConnection(_connectionString), MiniProfiler.Current))
            {
                return connection.ExecuteScalar<User>("exec mm_User_Read @ID", new { ID = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public void Add(User user)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public User GetByPasswordResetGUID(string guid)
        {
            throw new NotImplementedException();
        }
    }
}
