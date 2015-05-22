using MABMoney.Domain;

namespace MABMoney.Data.Abstract
{
    public interface IUserRepository
    {
        User Get();
        User Add(User user);
        User Update(User user);
        User Delete();

        User GetByEmailAddress(string email);
        User GetByPasswordResetGUID(string guid);
    }
}
