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
        private IRepository<Category_Budget, int> _categories_budgets;
        private IRepository<Transaction, int> _transactions;
        private IUnitOfWork _unitOfWork;

        public BudgetServices(IRepository<Budget, int> budgets, IRepository<Category_Budget, int> categories_budgets, IRepository<Transaction, int> transactions, IUnitOfWork unitOfWork)
        {
            _budgets = budgets;
            _categories_budgets = categories_budgets;
            _transactions = transactions;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BudgetDTO> All()
        {
            return _budgets.All().ToList().MapToList<BudgetDTO>();
        }

        public BudgetDTO Get(int id)
        {
            return MapBudget(_budgets.Get(id));
        }

        public BudgetDTO GetLatest(int accountId)
        {
            return MapBudget(_budgets.Query(x => x.Account_AccountID == accountId).OrderByDescending(x => x.BudgetID).FirstOrDefault());
        }

        private BudgetDTO MapBudget(Budget budget)
        {
            if (budget == null)
                return null;

            var transactions = _transactions.Query(x => x.Date >= budget.Start && x.Date < budget.End && x.Category.Account_AccountID == budget.Account_AccountID).ToList();

            var dto = budget.MapTo<BudgetDTO>();

            if (budget.Category_Budgets != null && budget.Category_Budgets.Count > 0)
            {
                dto.Category_Budgets = budget.Category_Budgets.ToList().Select(x => new Category_BudgetDTO
                {
                    Budget_BudgetID = x.Budget_BudgetID,
                    Category_CategoryID = x.Category_CategoryID,
                    Budget = x.Budget.MapTo<BudgetDTO>(),
                    Category = x.Category.MapTo<CategoryDTO>(),
                    Amount = x.Amount,
                    Total = transactions.Where(t => t.Category_CategoryID == x.Category_CategoryID).Sum(t => Math.Abs(t.Amount))
                }).ToList();
            }

            return dto;
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

        public void SaveCategoryBudget(Category_BudgetDTO dto)
        {
            var entity = _categories_budgets.Query(x => x.Budget_BudgetID == dto.Budget_BudgetID && x.Category_CategoryID == dto.Category_CategoryID)
                                            .FirstOrDefault();

            if (entity == null)
            {
                entity = dto.MapTo<Category_Budget>();
                _categories_budgets.Add(entity);
            }
            else
            {
                dto.MapTo(entity);
            }

            _unitOfWork.Commit();
        }

        public void DeleteCategoryBudget(int id)
        {
            _categories_budgets.Remove(id);
            _unitOfWork.Commit();
        }
    }
}
