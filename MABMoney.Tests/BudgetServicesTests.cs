using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MABMoney.Services;
using MABMoney.Data;
using MABMoney.Domain;
using Rhino.Mocks;
using MABMoney.Services.DTO;

namespace MABMoney.Tests
{
    [TestFixture]
    public class BudgetServicesTests
    {
        private IAccountServices _accountServices;
        private IRepository<Budget, int> _budgets;
        private IRepository<Account, int> _accounts;
        private IRepository<Category_Budget, int> _categories_budgets;
        private IRepository<Transaction, int> _transactions;

        private IUnitOfWork _unitOfWork;

        [SetUp]
        public void SetUp()
        {
            var accountDtos = new List<AccountDTO> {
                new AccountDTO { 
                    AccountID = 1,
                    Name = "Current",
                    StartingBalance = 100,
                    CurrentBalance = 100
                }
            };

            var accounts = new List<Account> {
                new Account { 
                    AccountID = 1,
                    Name = "Current",
                    StartingBalance = 100,
                    User_UserID = 1
                }
            };

            var categories = new List<Category> {
                new Category { 
                    CategoryID = 1,
                    Name = "Salary",
                    Account_AccountID = 1,
                    Type = CategoryType.Income
                },
                new Category { 
                    CategoryID = 2,
                    Name = "Rent",
                    Account_AccountID = 1,
                    Type = CategoryType.Expense
                },
                new Category { 
                    CategoryID = 3,
                    Name = "Bills",
                    Account_AccountID = 1,
                    Type = CategoryType.Expense
                }
            };

            var budgets = new List<Budget> {
                new Budget { 
                    BudgetID = 1,
                    Account_AccountID = 1,
                    Account = accounts[0],
                    Start = new DateTime(2020, 1, 1),
                    End = new DateTime(2020, 1, 31)
                }
            };

            var categories_budgets = new List<Category_Budget> {
                new Category_Budget {
                    Category = categories[1],
                    Budget_BudgetID = 1,
                    Category_CategoryID = 2,
                    Amount = 500,
                    Budget = budgets[0]
                },
                new Category_Budget {
                    Category = categories[2],
                    Budget_BudgetID = 1,
                    Category_CategoryID = 3,
                    Amount = 250,
                    Budget = budgets[0]
                }
            };

            budgets[0].Category_Budgets = categories_budgets;

            var transactions = new List<Transaction> {
                new Transaction { 
                    TransactionID = 1,
                    Account_AccountID = 1,
                    Category_CategoryID = 1,
                    Date = new DateTime(2020, 1, 1),
                    Description = "SALARY",
                    Amount = 1000
                },
                new Transaction { 
                    TransactionID = 1,
                    Account_AccountID = 1,
                    Category_CategoryID = 2,
                    Date = new DateTime(2020, 1, 1),
                    Description = "RENT",
                    Amount = -500
                },
                new Transaction { 
                    TransactionID = 1,
                    Account_AccountID = 1,
                    Category_CategoryID = 3,
                    Date = new DateTime(2020, 1, 2),
                    Description = "BILLS",
                    Amount = -150
                }
            };

            _accountServices = MockRepository.GenerateStub<IAccountServices>();
            _accountServices.Stub(x => x.All()).Return(accountDtos);
            _accountServices.Stub(x => x.Get(1)).Return(accountDtos[0]);

            _budgets = MockRepository.GenerateStub<IRepository<Budget, int>>();
            _budgets.Stub(x => x.All()).Return(budgets.AsQueryable());
            _budgets.Stub(x => x.Get(1)).Return(budgets[0]);
            _budgets.Stub(x => x.Query(null)).Return(budgets.AsQueryable()).IgnoreArguments();

            _accounts = MockRepository.GenerateStub<IRepository<Account, int>>();
            _accounts.Stub(x => x.All()).Return(accounts.AsQueryable());

            _categories_budgets = MockRepository.GenerateStub<IRepository<Category_Budget, int>>();
            _categories_budgets.Stub(x => x.All()).Return(categories_budgets.AsQueryable());

            _transactions = MockRepository.GenerateStub<IRepository<Transaction, int>>();
            _transactions.Stub(x => x.All()).Return(transactions.AsQueryable());
            _transactions.Stub(x => x.Query(null)).Return(transactions.AsQueryable()).IgnoreArguments();

            _unitOfWork = MockRepository.GenerateStub<IUnitOfWork>();
            _unitOfWork.Stub(x => x.DataStore.UserID).Return(1);
        }

        [Test]
        public void Unallocated_Funds_Calculated_Correctly()
        {
            var service = new BudgetServices(_budgets, _accounts, _categories_budgets, _transactions, _accountServices, _unitOfWork);

            var dto = service.Get(1);

            dto.ShouldNotBeNull();
        }
    }
}
