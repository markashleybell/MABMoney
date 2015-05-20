using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MABMoney.Services.DTO;
using MABMoney.Data;
using MABMoney.Domain;
using mab.lib.SimpleMapper;
using MABMoney.Data.Abstract;

namespace MABMoney.Services
{
    public class BudgetServices : IBudgetServices
    {
        private IBudgetRepository _budgets;
        private IAccountRepository _accounts;
        private ICategoryRepository _categories;
        private ICategory_BudgetRepository _categories_budgets;
        private ITransactionRepository _transactions;
        private IAccountServices _accountServices;
        private ICategoryServices _categoryServices;

        public BudgetServices(IBudgetRepository budgets, IAccountRepository accounts, ICategoryRepository categories, ICategory_BudgetRepository categories_budgets, ITransactionRepository transactions, IAccountServices accountServices, ICategoryServices categoryServices)
        {
            _budgets = budgets;
            _accounts = accounts;
            _categories = categories;
            _categories_budgets = categories_budgets;
            _transactions = transactions;
            _accountServices = accountServices;
            _categoryServices = categoryServices;
        }

        public IEnumerable<BudgetDTO> All()
        {
            return _budgets.Query(x => x.Account.User_UserID == _userId)
                           .Select(x => new BudgetDTO { 
                               BudgetID = x.BudgetID,
                               Account_AccountID = x.Account_AccountID,
                               Account = new AccountDTO { 
                                   Name = x.Account.Name
                               },
                               Start = x.Start,
                               End = x.End
                           })
                           .ToList();
        }

        public BudgetDTO Get(int id)
        {
            var q = _budgets.Query(x => x.Account.User_UserID == _userId && x.BudgetID == id)
                            .Select(x => new BudgetDTO {
                                BudgetID = x.BudgetID,
                                Account_AccountID = x.Account_AccountID,
                                Start = x.Start,
                                End = x.End
                            })
                            .FirstOrDefault();

            return MapBudget(q);
        }

        public BudgetDTO GetLatest(int accountId)
        {
            var now = _dateProvider.Now;
            var endOfToday = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            return MapBudget(_budgets.Query(x => x.Account.User_UserID == _userId && x.Account_AccountID == accountId && x.End >= endOfToday)
                                     .Select(x => new BudgetDTO { 
                                         BudgetID = x.BudgetID,
                                         Account_AccountID = x.Account_AccountID,
                                         Start = x.Start,
                                         End = x.End
                                     })
                                     .OrderByDescending(x => x.BudgetID)
                                     .FirstOrDefault());
        }

        private BudgetDTO MapBudget(BudgetDTO budget)
        {
            if (budget == null)
                return null;

            var balanceAtStart = _transactions.Query(x => x.Account_AccountID == budget.Account_AccountID && x.Date < budget.Start).Sum(x => x.Amount);
            var budgetTransactions = _transactions.Query(x => x.Account_AccountID == budget.Account_AccountID && x.Date >= budget.Start && x.Date < budget.End)
                                                  .Select(x => new TransactionDTO { 
                                                      //TransactionID = x.TransactionID,
                                                      //Date = x.Date,
                                                      //Description = x.Description,
                                                      //Note = x.Note,
                                                      Amount = x.Amount,
                                                      Category_CategoryID = x.Category_CategoryID,
                                                      //Account_AccountID = x.Account_AccountID,
                                                      //TransferGUID = x.TransferGUID
                                                  })
                                                  .ToList();

            // Get any categories which aren't deleted or which were deleted after this budget period ended
            var category_budgets = _categories_budgets.QueryWithIncludes(x => x.Budget_BudgetID == budget.BudgetID && (!x.Category.Deleted || (x.Category.Deleted && x.Category.DeletedDate > budget.End)), "Category")
                                                      .Select(x => new Category_BudgetDTO {
                                                          Budget_BudgetID = x.Budget_BudgetID,
                                                          Category_CategoryID = x.Category_CategoryID,
                                                          Category = new CategoryDTO { 
                                                              CategoryID = x.Category.CategoryID,
                                                              Name = x.Category.Name,
                                                              Type = (CategoryTypeDTO)x.Category.Type
                                                          },
                                                          Amount = x.Amount
                                                      })
                                                      .ToList();

            if (category_budgets.Count > 0)
            {
                // Assign the categories for this budget
                budget.Category_Budgets = category_budgets;
                // Work out the total spent in each category so far
                budget.Category_Budgets.ForEach(x => x.Total = budgetTransactions.Where(t => t.Category_CategoryID == x.Category_CategoryID).Sum(t => Math.Abs(t.Amount)));

                // Work out the total amount overspent across all categories
                var overspend = budget.Category_Budgets.Where(x => x.Total > x.Amount).Select(x => x.Total - x.Amount).Sum();

                // Work out how much money has been allocated to budget categories
                var allocated = budget.Category_Budgets.Sum(x => x.Amount);

                // Work out how much money has been spent in budget categories
                var account = _accountServices.Get(budget.Account_AccountID);
                var allocatedSpent = budget.Category_Budgets.Sum(x => x.Total);

                var balanceAtBudgetStart = balanceAtStart + account.StartingBalance;

                var unallocatedAmount = ((balanceAtBudgetStart - allocated) - overspend) + budgetTransactions.Where(x => x.Amount > 0).Sum(x => x.Amount);

                // Work out how much money was spent in transactions not assigned to a category
                var unallocatedSpent = budgetTransactions.Where(x => x.Amount < 0 && x.Category_CategoryID == null).ToList().Sum(x => Math.Abs(x.Amount));

                // If there is money left over after all budget category amounts and any overspend have been subtracted
                if (unallocatedAmount > 0)
                {
                    // Show how much and how much we've spent so far
                    budget.Category_Budgets.Add(new Category_BudgetDTO {
                        Budget_BudgetID = budget.BudgetID,
                        Category_CategoryID = 0,
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

            return budget;
        }

        public void Save(BudgetDTO dto)
        {
            // Try and get the budget
            var budget = _budgets.Get(dto.BudgetID);

            // If the budget exists AND belongs to this user
            if (budget != null && budget.Account.User_UserID == _userId)
            {
                // Update the budget
                dto.MapTo(budget);
                _unitOfWork.Commit();
            }
            else
            {
                // Add the budget
                budget = dto.MapTo<Budget>();
                _budgets.Add(budget);
                _unitOfWork.Commit();

                // Update the DTO with the new ID
                dto.BudgetID = budget.BudgetID;
            }
        }

        public void Delete(int id)
        {
            var budget = _budgets.Query(x => x.Account.User_UserID == _userId && x.BudgetID == id).FirstOrDefault();

            if(budget != null)
            {
                _budgets.Remove(id);
                _unitOfWork.Commit();
            }
        }

        public void SaveCategoryBudget(Category_BudgetDTO dto)
        {
            _categoryServices.Save(dto.Category);

            dto.Category_CategoryID = dto.Category.CategoryID;

            var entity = _categories_budgets.Query(x => x.Budget.Account.User_UserID == _userId && x.Budget_BudgetID == dto.Budget_BudgetID && x.Category_CategoryID == dto.Category_CategoryID)
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
            throw new NotImplementedException();
        }

        public int GetBudgetCount(int accountId)
        {
            return _budgets.Query(x => x.Account_AccountID == accountId).Count();
        }
    }
}
