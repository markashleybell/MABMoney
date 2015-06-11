using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Domain;
using MABMoney.Data;
using MABMoney.Services.DTO;
using mab.lib.SimpleMapper;
using MABMoney.Data.Abstract;

namespace MABMoney.Services
{
    public class UserServices : IUserServices
    {
        private IUserRepository _users;
        
        public UserServices(IUserRepository users)
        {
            _users = users;
        }

        public UserDTO Get(int id)
        {
            return _users.Get(id).MapTo<UserDTO>();
        }

        public UserDTO GetByEmailAddress(string email)
        {
            return _users.GetByEmailAddress(email).MapTo<UserDTO>();
        }

        public void Save(UserDTO dto)
        {
            var user = _users.Get(dto.UserID);

            if (user != null)
            {
                dto.MapTo(user);
                _users.Update(user);
            }
            else
            {
                user = _users.Add(dto.MapTo<User>());
                dto.UserID = user.UserID;
            }
        }

        public void ResetPassword(string guid, string newPassword)
        {
            var user = _users.GetByPasswordResetGUID(guid);

            if(user != null)
            {
                user.Password = newPassword;
                user.PasswordResetGUID = null;
                user.PasswordResetExpiry = null;

                _users.Update(user);
            }
        }

        public void Delete(int id)
        {
            _users.Delete(id);
        }

        public UserDTO GetByPasswordResetGUID(string guid)
        {
            return _users.GetByPasswordResetGUID(guid).MapTo<UserDTO>();
        }
    }
}
