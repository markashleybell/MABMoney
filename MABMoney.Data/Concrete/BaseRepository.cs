using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace MABMoney.Data.Concrete
{
    public class BaseRepository
    {
        protected string _connectionString;
        protected int _userId;

        public BaseRepository(string connectionString, int userId)
        {
            _connectionString = connectionString;
            _userId = userId;
        }

        protected T GetSingle<T>(string procedureName, object param)
        {
            using (var connection = new ProfiledDbConnection(new SqlConnection(_connectionString), MiniProfiler.Current))
            {
                return connection.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
        }

        protected IEnumerable<T> GetEnumerable<T>(string procedureName, object param)
        {
            using (var connection = new ProfiledDbConnection(new SqlConnection(_connectionString), MiniProfiler.Current))
            {
                return connection.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }

        protected T AddOrUpdate<T>(string procedureName, object param)
        {
            using (var connection = new ProfiledDbConnection(new SqlConnection(_connectionString), MiniProfiler.Current))
            {
                return connection.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure).Single();
            }
        }
    }
}
