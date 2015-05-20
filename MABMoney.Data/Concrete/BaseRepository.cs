using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
