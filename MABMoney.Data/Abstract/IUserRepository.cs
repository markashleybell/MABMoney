using MABMoney.Domain;

namespace MABMoney.Data.Abstract
{
    public interface IUserRepository
    {
        User Get();
        void Add(User user);
        void Update(User user);
        void Delete();

        User GetByEmailAddress(string email);
        User GetByPasswordResetGUID(string guid);
    }
}
