using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MABMoney.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDataStoreFactory _dataStoreFactory;
        private IDataStore _dataStore;

        public UnitOfWork(IDataStoreFactory dataStoreFactory)
        {
            _dataStoreFactory = dataStoreFactory;
        }

        public IDataStore DataStore
        {
            get { return _dataStore ?? (_dataStore = _dataStoreFactory.Get()); }
        }

        public void Commit()
        {
            DataStore.Commit();
        }
    }
}
