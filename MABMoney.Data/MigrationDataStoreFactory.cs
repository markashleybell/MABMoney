using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Data
{
    public class MigrationDataStoreFactory : IDbContextFactory<DataStore>
    {
        public DataStore Create()
        {
            return new DataStore(0, new DateTimeProvider(() => DateTime.Now));
        }
    }
}
