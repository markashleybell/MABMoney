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
        private IUnitOfWork _unitOfWork;
        private IDateTimeProvider _dateProvider;

        private int _userId;

        public UserServices(IRepository<User, int> users, IUnitOfWork unitOfWork, IDateTimeProvider dateProvider)
        {
            _users = users;
            _unitOfWork = unitOfWork;
            _dateProvider = dateProvider;
            _userId = unitOfWork.DataStore.UserID;
        }

        public IEnumerable<UserDTO> All()
        {
            return _users.All().ToList().MapToList<UserDTO>();
        }

        public UserDTO Get(int id)
        {
            return MapUser(_users.Get(id));
            // return MapUser(_users.QueryWithIncludes(x => x.UserID == id, "Accounts").FirstOrDefault());
        }

        public UserDTO GetByEmailAddress(string email)
        {
            return MapUser(_users.Query(x => x.Email == email).FirstOrDefault());
        }

        private UserDTO MapUser(User user)
        {
            if (user == null)
                return null;

            var dto = user.MapTo<UserDTO>();

            if (user.Accounts != null)
                dto.Accounts = user.Accounts.ToList().MapToList<AccountDTO>();

            return dto;
        }

        public void Save(UserDTO dto)
        {
            if (dto.UserID == 0)
            {
                var entity = dto.MapTo<User>();
                _users.Add(entity);
                _unitOfWork.Commit();
                dto.UserID = entity.UserID;
            }
            else
            {
                var entity = _users.Get(dto.UserID);

                // Store the existing password
                var oldPassword = entity.Password;

                dto.MapTo(entity);

                // If a new password hasn't been supplied, keep the old one
                if (entity.Password == null)
                    entity.Password = oldPassword;

                _unitOfWork.Commit();
            }
        }

        public void Delete(int id)
        {
            _users.Remove(id);
            _unitOfWork.Commit();
        }

        public UserDTO GetByPasswordResetGUID(string guid)
        {
            return MapUser(_users.Query(x => x.PasswordResetGUID == guid && x.PasswordResetExpiry >= _dateProvider.Now).FirstOrDefault());
        }
    }
}
