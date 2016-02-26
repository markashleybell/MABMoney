using System.Collections.Generic;

namespace MABMoney.Domain.Abstract
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
