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

        public CachingUserServices(IUserServices nonCachingUserServices, IModelCache cache, IModelCacheConfiguration cacheConfig) 
            : base(cache, cacheConfig) 
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
                "user-minimal-" + id,
                CacheExpiry.OneHour,
                () => _userServices.GetMinimal(id),
                "user", "all"
            );
        }

        public UserDTO GetByEmailAddress(string email)
        {
            throw new NotImplementedException();
        }

        public UserDTO GetByPasswordResetGUID(string guid)
        {
            throw new NotImplementedException();
        }

        public void Save(UserDTO dto)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
