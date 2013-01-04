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

        public UserServices(IRepository<User, int> users, IUnitOfWork unitOfWork)
        {
            _users = users;
            _unitOfWork = unitOfWork;
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
                dto.MapTo(entity);
                _unitOfWork.Commit();
            }
        }

        public void Delete(int id)
        {
            _users.Remove(id);
            _unitOfWork.Commit();
        }
    }
}
