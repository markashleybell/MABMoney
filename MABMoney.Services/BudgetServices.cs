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
        private IRepository<Account, int> _accounts;
        private IRepository<Category_Budget, int> _categories_budgets;
        private IRepository<Transaction, int> _transactions;
        private IUnitOfWork _unitOfWork;

        public BudgetServices(IRepository<Budget, int> budgets, IRepository<Account, int> accounts, IRepository<Category_Budget, int> categories_budgets, IRepository<Transaction, int> transactions, IUnitOfWork unitOfWork)
        {
            _budgets = budgets;
            _accounts = accounts;
            _categories_budgets = categories_budgets;
            _transactions = transactions;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BudgetDTO> All(int userId)
        {
            return _budgets.Query(x => x.Account.User_UserID == userId).ToList().MapToList<BudgetDTO>();
        }

        public BudgetDTO Get(int userId, int id)
        {
            return MapBudget(_budgets.Query(x => x.Account.User_UserID == userId && x.BudgetID == id).FirstOrDefault());
        }

        public BudgetDTO GetLatest(int userId, int accountId)
        {
            return MapBudget(_budgets.Query(x => x.Account.User_UserID == userId && x.Account_AccountID == accountId).OrderByDescending(x => x.BudgetID).FirstOrDefault());
        }

        private BudgetDTO MapBudget(Budget budget)
        {
            if (budget == null)
                return null;

            var transactions = _transactions.Query(x => x.Date >= budget.Start && x.Date < budget.End && x.Account_AccountID == budget.Account_AccountID).ToList();

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

                // Work out the total amount overspent across all categories
                var overspend = dto.Category_Budgets.Where(x => x.Total > x.Amount).Select(x => x.Total - x.Amount).Sum();

                // Get the total income since the budget start date
                var incomeSinceBudgetStart = transactions.Where(x => x.Amount > 0).ToList().Sum(x => x.Amount);
                
                // Work out how much money has been allocated to budget categories
                var allocated = dto.Category_Budgets.Sum(x => x.Amount);

                // Work out how much money is not allocated to any budget category (disposable income)
                var unallocatedAmount = (incomeSinceBudgetStart - allocated) - overspend;

                // Work out how much money was spent in transactions not assigned to a category
                var unallocatedSpent = transactions.Where(x => x.Amount < 0 && x.Category_CategoryID == null).ToList().Sum(x => Math.Abs(x.Amount));

                // If there is money left over after all budget category amounts and any overspend have been subtracted
                if (incomeSinceBudgetStart > allocated)
                {
                    // Show how much and how much we've spent so far
                    dto.Category_Budgets.Add(new Category_BudgetDTO
                    {
                        Budget_BudgetID = budget.BudgetID,
                        Category_CategoryID = 0,
                        Budget = budget.MapTo<BudgetDTO>(),
                        Category = new CategoryDTO
                        {
                            CategoryID = 0,
                            Name = "Unallocated",
                            Type = CategoryTypeDTO.Expense
                        },
                        Amount = unallocatedAmount,
                        Total = unallocatedSpent
                    });
                }
            }

            return dto;
        }

        public void Save(int userId, BudgetDTO dto)
        {
            var account = _accounts.Get(dto.Account_AccountID);

            if(account != null && account.User_UserID == userId)
            {
                if (dto.BudgetID == 0)
                {
                    var entity = dto.MapTo<Budget>();
                    _budgets.Add(entity);
                    _unitOfWork.Commit(userId);
                    dto.BudgetID = entity.BudgetID;
                }
                else
                {
                    var entity = _budgets.Get(dto.BudgetID);
                    dto.MapTo(entity);
                    _unitOfWork.Commit(userId);
                }
            }
        }

        public void Delete(int userId, int id)
        {
            var budget = _budgets.Query(x => x.Account.User_UserID == userId && x.BudgetID == id).FirstOrDefault();

            if(budget != null)
            {
                _budgets.Remove(id);
                _unitOfWork.Commit(userId);
            }
        }

        public void SaveCategoryBudget(int userId, Category_BudgetDTO dto)
        {
            var entity = _categories_budgets.Query(x => x.Budget.Account.User_UserID == userId && x.Budget_BudgetID == dto.Budget_BudgetID && x.Category_CategoryID == dto.Category_CategoryID)
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

            _unitOfWork.Commit(userId);
        }

        public void DeleteCategoryBudget(int userId, int id)
        {
            throw new NotImplementedException();
        }
    }
}
