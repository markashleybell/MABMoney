using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace MABMoney.Data.Concrete
{
    public abstract class BaseRepository
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

        protected T GetScalar<T>(string procedureName, object param)
        {
            using (var connection = new ProfiledDbConnection(new SqlConnection(_connectionString), MiniProfiler.Current))
            {
                return connection.ExecuteScalar<T>(procedureName, param, commandType: CommandType.StoredProcedure);
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

        protected void Execute(string procedureName, object param)
        {
            using (var connection = new ProfiledDbConnection(new SqlConnection(_connectionString), MiniProfiler.Current))
            {
                connection.Execute(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
