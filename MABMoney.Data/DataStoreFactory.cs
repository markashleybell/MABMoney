using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MABMoney.Data
{
    public class DataStoreFactory : Disposable, IDataStoreFactory
    {
        private DataStore _database;
        private int _userId;
        private IDateTimeProvider _dateTimeServices;
        private string _connectionString;

        public DataStoreFactory(int userId, IDateTimeProvider dateTimeServices)
        {
            _userId = userId;
            _dateTimeServices = dateTimeServices;
        }

        public DataStoreFactory(int userId, IDateTimeProvider dateTimeServices, string connectionString)
        {
            _userId = userId;
            _dateTimeServices = dateTimeServices;
            _connectionString = connectionString;
        }

        public IDataStore Get()
        {
            if(_connectionString != null)
                _database = new DataStore(_userId, _dateTimeServices, _connectionString);
            else
                _database = new DataStore(_userId, _dateTimeServices);

            return _database;
        }

        protected override void DisposeCore()
        {
            if (_database != null) _database.Dispose();
        }
    }
}
