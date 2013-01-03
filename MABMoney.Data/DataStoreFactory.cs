using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MABMoney.Data
{
    public class DataStoreFactory : Disposable, IDataStoreFactory
    {
        private DataStore _database;

        public IDataStore Get()
        {
            _database = new DataStore();
            return _database;
        }

        protected override void DisposeCore()
        {
            if (_database != null) _database.Dispose();
        }
    }
}
