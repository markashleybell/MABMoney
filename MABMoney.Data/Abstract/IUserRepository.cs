using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MABMoney.Domain;

namespace MABMoney.Data.Abstract
{
    public interface IUserRepository
    {
        List<User> All();

        User Get(int id);
        void Add(User user);
        void Update(User user);
        void Delete(int id);

        User GetByEmailAddress(string email);
        User GetByPasswordResetGUID(string guid);
    }
}
