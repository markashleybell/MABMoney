namespace MABMoney.Domain.Abstract
{
    public interface IUserRepository
    {
        User Get(int id);
        User Add(User user);
        User Update(User user);
        User Delete(int id);

        User GetByEmailAddress(string email);
        User GetByPasswordResetGUID(string guid);
    }
}
