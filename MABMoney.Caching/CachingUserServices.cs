using MABMoney.Services;
using MABMoney.Services.DTO;

namespace MABMoney.Caching
{
    public class CachingUserServices : CachingServicesBase, IUserServices
    {
        private IUserServices _userServices;

        public CachingUserServices(IUserServices nonCachingUserServices, IModelCache cache, ICachingHelpers helpers)
            : base(cache, helpers) 
        {
            _userServices = nonCachingUserServices;
        }

        public UserDTO Get(int id)
        {
            return CacheAndGetValue<UserDTO>(
                "user-full",
                CacheExpiry.OneHour,
                () => _userServices.Get(id),
                "user", "account", "category", "all"
            );
        }

        public UserDTO GetByEmailAddress(string email)
        {
            return _userServices.GetByEmailAddress(email);
        }

        public UserDTO GetByPasswordResetGUID(string guid)
        {
            return _userServices.GetByPasswordResetGUID(guid);
        }

        public void Save(UserDTO dto)
        {
            _userServices.Save(dto);
        }

        public void ResetPassword(string guid, string newPassword)
        {
            _userServices.ResetPassword(guid, newPassword);
        }

        public void Delete(int id)
        {
            _userServices.Delete(id);
        }
    }
}
