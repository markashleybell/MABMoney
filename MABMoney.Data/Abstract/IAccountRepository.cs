using MABMoney.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Data.Abstract
{
    public interface IAccountRepository
    {
        List<Account> All();
        Account Get(int id);
        void Add(Account account);
        void Update(Account account);
        void Delete(int id);
    }
}
