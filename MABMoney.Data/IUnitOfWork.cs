using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MABMoney.Data
{
    public interface IUnitOfWork
    {
        IDataStore DataStore { get; }
        void Commit();
    }
}
