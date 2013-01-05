using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;
using MABMoney.Data;
using MABMoney.Domain;
using mab.lib.SimpleMapper;

namespace MABMoney.Services
{
    public class AccountServices : IAccountServices
    {
        private IRepository<Account, int> _accounts;
        private IUnitOfWork _unitOfWork;

        public AccountServices(IRepository<Account, int> accounts, IUnitOfWork unitOfWork)
        {
            _accounts = accounts;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<AccountDTO> All()
        {
            return _accounts.All().ToList().MapToList<AccountDTO>();
        }

        public AccountDTO Get(int id)
        {
            return _accounts.Get(id).MapTo<AccountDTO>();
        }

        public void Save(AccountDTO dto)
        {
            if (dto.AccountID == 0)
            {
                var entity = dto.MapTo<Account>();
                _accounts.Add(entity);
                _unitOfWork.Commit();
                dto.AccountID = entity.AccountID;
            }
            else
            {
                var entity = _accounts.Get(dto.AccountID);
                dto.MapTo(entity);
                _unitOfWork.Commit();
            }
        }

        public void Delete(int id)
        {
            _accounts.Remove(id);
            _unitOfWork.Commit();
        }
    }
}
