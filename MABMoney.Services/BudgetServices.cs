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
    public class BudgetServices : IBudgetServices
    {
        private IRepository<Budget, int> _budgets;
        private IUnitOfWork _unitOfWork;

        public BudgetServices(IRepository<Budget, int> budgets, IUnitOfWork unitOfWork)
        {
            _budgets = budgets;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BudgetDTO> All()
        {
            return _budgets.All().ToList().MapToList<BudgetDTO>();
        }

        public BudgetDTO Get(int id)
        {
            return _budgets.Get(id).MapTo<BudgetDTO>();
        }

        public void Save(BudgetDTO dto)
        {
            if (dto.BudgetID == 0)
            {
                var entity = dto.MapTo<Budget>();
                _budgets.Add(entity);
                _unitOfWork.Commit();
                dto.BudgetID = entity.BudgetID;
            }
            else
            {
                var entity = _budgets.Get(dto.BudgetID);
                dto.MapTo(entity);
                _unitOfWork.Commit();
            }
        }

        public void Delete(int id)
        {
            _budgets.Remove(id);
            _unitOfWork.Commit();
        }
    }
}
