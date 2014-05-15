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

        public IEnumerable<UserDTO> All()
        {
            return _userServices.All();
        }

        public UserDTO Get(int id)
        {
            return _userServices.Get(id);
        }

        public UserDTO GetMinimal(int id)
        {
            return CacheAndGetValue<UserDTO>(
                "user-minimal",
                CacheExpiry.OneHour,
                () => _userServices.GetMinimal(id),
                "user", "all"
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

        public void Delete(int id)
        {
            _userServices.Delete(id);
        }
    }
}
