using mab.lib.SimpleMapper;
using MABMoney.Data.Abstract;
using MABMoney.Domain;

namespace MABMoney.Data.Concrete
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(string connectionString, int userId) : base(connectionString, userId) { }

        public User Get()
        {
            return GetSingle<User>("mm_Users_Read", new { UserID = _userId });
        }

        public void Add(User user)
        {
            var result = AddOrUpdate<User>("mm_Users_Create", new {
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                Password = user.Password,
                IsAdmin = user.IsAdmin
            });

            result.MapTo(user);
        }

        public void Update(User user)
        {
            var result = AddOrUpdate<User>("mm_Users_Update", new {
                UserID = _userId,
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                Password = user.Password,
                IsAdmin = user.IsAdmin
            });

            result.MapTo(user);
        }

        public void Delete()
        {
            Execute("mm_Users_Delete", new { UserID = _userId });
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
