﻿using mab.lib.SimpleMapper;
using MABMoney.Domain;
using MABMoney.Domain.Abstract;
using MABMoney.Services.DTO;

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
            // If user ID is 0, someone's explicitly trying to create a 
            // new user record, which is fine, so let them do it
            if (dto.UserID == 0)
            {
                var user = _users.Add(dto.MapTo<User>());
                dto.UserID = user.UserID;
            }
            else
            {
                // If an ID has been passed in, but it's *not* the same ID
                // which was retrieved from the user cookie and passed to 
                // the repository constructor, someone other than that user
                // is attempting to update the user record...
                // If that is the case, the repository Get method will return 
                // null and we don't perform the update
                var user = _users.Get(dto.UserID);

                if (user != null)
                {
                    dto.MapTo(user);
                    _users.Update(user);
                }
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
