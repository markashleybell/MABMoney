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
        IEnumerable<Account> All();
        Account Get(int id);
        Account Add(Account account);
        Account Update(Account account);
        Account Delete(int id);
    }
}
