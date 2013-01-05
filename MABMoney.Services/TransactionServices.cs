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
    public class TransactionServices : ITransactionServices
    {
        private IRepository<Transaction, int> _transactions;
        private IUnitOfWork _unitOfWork;

        public TransactionServices(IRepository<Transaction, int> transactions, IUnitOfWork unitOfWork)
        {
            _transactions = transactions;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<TransactionDTO> All()
        {
            return _transactions.All().ToList().MapToList<TransactionDTO>();
        }

        public TransactionDTO Get(int id)
        {
            return _transactions.Get(id).MapTo<TransactionDTO>();
        }

        public void Save(TransactionDTO dto)
        {
            if (dto.TransactionID == 0)
            {
                var entity = dto.MapTo<Transaction>();
                _transactions.Add(entity);
                _unitOfWork.Commit();
                dto.TransactionID = entity.TransactionID;
            }
            else
            {
                var entity = _transactions.Get(dto.TransactionID);
                dto.MapTo(entity);
                _unitOfWork.Commit();
            }
        }

        public void Delete(int id)
        {
            _transactions.Remove(id);
            _unitOfWork.Commit();
        }
    }
}
