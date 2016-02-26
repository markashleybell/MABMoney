using MABMoney.Domain;
using MABMoney.Domain.Abstract;

namespace MABMoney.Data.Concrete
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(string connectionString, int userId) : base(connectionString, userId) { }

        public User Get(int id)
        {
            if (id != _userId)
                return null;

            return GetSingle<User>("mm_Users_Read", new { UserID = id });
        }

        public User Add(User user)
        {
            return AddOrUpdate<User>("mm_Users_Create", new {
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                Password = user.Password
            });
        }

        public User Update(User user)
        {
            if (user.UserID != _userId)
                return null;

            return AddOrUpdate<User>("mm_Users_Update", new {
                UserID = user.UserID,
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                Password = user.Password,
                PasswordResetGUID = user.PasswordResetGUID,
                PasswordResetExpiry = user.PasswordResetExpiry
            });
        }

        public User Delete(int id)
        {
            if (id != _userId)
                return null;

            return AddOrUpdate<User>("mm_Users_Delete", new { UserID = id });
        }

        public User GetByEmailAddress(string email)
        {
            return GetSingle<User>("mm_Users_GetByEmailAddress", new { Email = email });
        }

        public User GetByPasswordResetGUID(string guid)
        {
            return GetSingle<User>("mm_Users_GetByPasswordResetGUID", new { GUID = guid });
        }
    }
}
