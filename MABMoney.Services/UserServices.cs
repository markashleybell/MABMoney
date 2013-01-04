using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Domain;
using MABMoney.Data;
using MABMoney.Services.DTO;
using mab.lib.SimpleMapper;

namespace MABMoney.Services
{
    public class UserServices : IUserServices
    {
        private IRepository<User, int> _users;

        public UserServices(IRepository<User, int> users)
        {
            _users = users;
        }

        public IEnumerable<UserDTO> All()
        {
            return _users.All().ToList().MapToList<UserDTO>();
        }

        public UserDTO Get(int id)
        {
            return _users.Get(id).MapTo<UserDTO>();
        }

        public UserDTO GetByEmailAddress(string email)
        {
            return _users.Query(x => x.Email == email).FirstOrDefault().MapTo<UserDTO>();
        }

        public void Save(UserDTO dto)
        {
            var entity = dto.MapTo<User>();
            _users.Add(entity);
        }

        public void Delete(int id)
        {
            _users.Remove(id);
        }
    }
}
