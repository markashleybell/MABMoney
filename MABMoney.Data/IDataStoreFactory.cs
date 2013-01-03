using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MABMoney.Data
{
    public interface IDataStoreFactory
    {
        IDataStore Get();
    }
}
