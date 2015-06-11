using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public UserDTO Get()
        {
            return CacheAndGetValue<UserDTO>(
                "user-full",
                CacheExpiry.OneHour,
                () => _userServices.Get(),
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

        public void Delete()
        {
            _userServices.Delete();
        }
    }
}
