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
            return MapUser(_users.Get());
        }

        public UserDTO GetByEmailAddress(string email)
        {
            return MapUser(_users.GetByEmailAddress(email));
        }

        private UserDTO MapUser(User user)
        {
            if (user == null)
                return null;

            var dto = user.MapTo<UserDTO>();

            //if (user.Accounts != null)
            //{
            //    dto.Accounts = new List<AccountDTO>();

            //    foreach (var account in user.Accounts)
            //    {
            //        var accountModel = account.MapTo<AccountDTO>();
            //        accountModel.Categories = account.Categories.ToList().MapToList<CategoryDTO>();

            //        dto.Accounts.Add(accountModel);
            //    }
            //}

            return dto;
        }

        public void Save(UserDTO dto)
        {
            if (dto.UserID == 0)
            {
                var user = _users.Add(dto.MapTo<User>());
                dto.UserID = user.UserID;
            }
            else
            {
                throw new NotImplementedException();

                //var user = _users.Get(dto.UserID);

                //// Store the existing password
                //var oldPassword = user.Password;

                //dto.MapTo(entity);

                //// If a new password hasn't been supplied, keep the old one
                //if (entity.Password == null)
                //    entity.Password = oldPassword;
            }
        }

        public void Delete()
        {
            _users.Delete();
        }

        public UserDTO GetByPasswordResetGUID(string guid)
        {
            return MapUser(_users.GetByPasswordResetGUID(guid));
        }
    }
}
