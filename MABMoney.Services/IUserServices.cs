using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Domain;

namespace MABMoney.Services
{
    public interface IUserServices
    {
        User GetUserByID(int id);
        User GetUserByEmailAddress(string email);
        IEnumerable<User> GetAllUsers();
    }
}
