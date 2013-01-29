﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MABMoney.Services;
using System.Web;
using MABMoney.Web.Infrastructure;
using MABMoney.Web.Models;
using MABMoney.Web.Models.Home;
using MABMoney.Services.DTO;
using Rhino.Mocks;
using MABMoney.Web.Controllers;
using System.Web.Mvc;

namespace MABMoney.Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private IUserServices _userServices;
        private IAccountServices _accountServices;
        private ICategoryServices _categoryServices;
        private ITransactionServices _transactionServices;
        private IBudgetServices _budgetServices;
        private HttpContextBase _context;
        private ISiteConfiguration _config;
        private ProfileViewModel _profile;
        private IDateTimeProvider _dateProvider;

        [SetUp]
        public void SetUp()
        {
            var accounts = new List<AccountDTO> {
                new AccountDTO { 
                    AccountID = 1,
                    Name = "Current",
                    StartingBalance = 0,
                    User_UserID = 1
                },
                new AccountDTO { 
                    AccountID = 2,
                    Name = "Savings",
                    StartingBalance = 500,
                    User_UserID = 1
                }
            };

            var categories = new List<CategoryDTO> {
                new CategoryDTO { 
                    CategoryID = 1,
                    Name = "Rent",
                    Account_AccountID = 1,
                    Type = CategoryTypeDTO.Expense
                },
                new CategoryDTO { 
                    CategoryID = 2,
                    Name = "Salary",
                    Account_AccountID = 1,
                    Type = CategoryTypeDTO.Income
                },
                new CategoryDTO { 
                    CategoryID = 3,
                    Name = "Fuel",
                    Account_AccountID = 1,
                    Type = CategoryTypeDTO.Expense
                },
                new CategoryDTO { 
                    CategoryID = 4,
                    Name = "Food",
                    Account_AccountID = 1,
                    Type = CategoryTypeDTO.Expense
                },
                new CategoryDTO { 
                    CategoryID = 5,
                    Name = "Deposit",
                    Account_AccountID = 2,
                    Type = CategoryTypeDTO.Income
                }
            };

            var budgets = new List<BudgetDTO> {
                new BudgetDTO { 
                    BudgetID = 1,
                    Account_AccountID = 1,
                    Start = new DateTime(2020, 1, 1),
                    End = new DateTime(2020, 1, 31),
                    Category_Budgets = new List<Category_BudgetDTO> {
                        new Category_BudgetDTO {
                            Category = categories[0],
                            Budget_BudgetID = 1,
                            Category_CategoryID = 1,
                            Amount = 500,
                            Total = 0
                        },
                        new Category_BudgetDTO {
                            Category = categories[2],
                            Budget_BudgetID = 1,
                            Category_CategoryID = 3,
                            Amount = 250,
                            Total = 0
                        },
                        new Category_BudgetDTO {
                            Category = categories[3],
                            Budget_BudgetID = 1,
                            Category_CategoryID = 4,
                            Amount = 250,
                            Total = 0
                        }
                    }
                },
                new BudgetDTO { 
                    BudgetID = 2,
                    Account_AccountID = 2,
                    Start = new DateTime(2021, 1, 1),
                    End = new DateTime(2021, 1, 31),
                    Category_Budgets = new List<Category_BudgetDTO> {
                        new Category_BudgetDTO {
                            Category = categories[3],
                            Budget_BudgetID = 2,
                            Category_CategoryID = 4,
                            Amount = 50,
                            Total = 0
                        }
                    }
                }
            };

            var transactions = new List<TransactionDTO> {
                new TransactionDTO { 
                    TransactionID = 1,
                    Category_CategoryID = 2,
                    Account_AccountID = 1,
                    Date = new DateTime(2020, 1, 1),
                    Amount = 1000.00M,
                    Description = "SALARY"
                },
                new TransactionDTO { 
                    TransactionID = 2,
                    Category_CategoryID = 3,
                    Account_AccountID = 1,
                    Date = new DateTime(2020, 1, 2),
                    Amount = -500.00M,
                    Description = "FUEL"
                },
                new TransactionDTO { 
                    TransactionID = 3,
                    Category_CategoryID = 4,
                    Account_AccountID = 1,
                    Date = new DateTime(2020, 1, 3),
                    Amount = -100.00M,
                    Description = "FOOD"
                },
                new TransactionDTO { 
                    TransactionID = 4,
                    Category_CategoryID = 5,
                    Account_AccountID = 2,
                    Date = new DateTime(2020, 1, 4),
                    Amount = 100.00M,
                    Description = "DEPOSIT"
                }
            };

            _profile = new ProfileViewModel
            {
                UserID = 1
            };

            var user = new UserDTO {
                UserID = 1,
                Forename = "TEST",
                Surname = "USER",
                Email = "testuser@test.com",
                Accounts = accounts
            };

            _accountServices = MockRepository.GenerateStub<IAccountServices>();
            _accountServices.Stub(x => x.All(1)).Return(accounts.Where(x => x.User_UserID == 1));
            _accountServices.Stub(x => x.Get(1, 1)).Return(accounts[0]);
            _accountServices.Stub(x => x.Get(1, 2)).Return(accounts[1]);

            _categoryServices = MockRepository.GenerateStub<ICategoryServices>();
            _categoryServices.Stub(x => x.All(1)).Return(categories);
            _categoryServices.Stub(x => x.Get(1, 1)).Return(categories[0]);
            _categoryServices.Stub(x => x.Get(1, 2)).Return(categories[1]);
            _categoryServices.Stub(x => x.Get(1, 3)).Return(categories[2]);
            _categoryServices.Stub(x => x.Get(1, 4)).Return(categories[3]);
            _categoryServices.Stub(x => x.Get(1, 5)).Return(categories[4]);

            _transactionServices = MockRepository.GenerateStub<ITransactionServices>();
            _transactionServices.Stub(x => x.All(1)).Return(transactions);
            _transactionServices.Stub(x => x.Get(1, 1)).Return(transactions[0]);
            _transactionServices.Stub(x => x.Get(1, 2)).Return(transactions[1]);
            _transactionServices.Stub(x => x.Get(1, 3)).Return(transactions[2]);

            _budgetServices = MockRepository.GenerateStub<IBudgetServices>();
            _budgetServices.Stub(x => x.All(1)).Return(budgets);
            _budgetServices.Stub(x => x.Get(1, 1)).Return(budgets[0]);
            _budgetServices.Stub(x => x.Get(1, 2)).Return(budgets[1]);
            _budgetServices.Stub(x => x.GetLatest(1, 1)).Return(budgets[0]);

            _userServices = MockRepository.GenerateStub<IUserServices>();
            _userServices.Stub(x => x.Get(1)).Return(user);

            _context = MockRepository.GenerateStub<HttpContextBase>();

            _config = MockRepository.GenerateStub<ISiteConfiguration>();

            _config.Stub(x => x.SharedSecret).Return("SHAREDSECRET");

            _dateProvider = MockRepository.GenerateStub<IDateTimeProvider>();
            _dateProvider.Stub(x => x.Date).Return(new DateTime(2020, 1, 1));
        }

        [Test]
        public void Index_Get()
        {
            var controller = new HomeController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

            var result = controller.Index(_profile) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as IndexViewModel;

            model.ShouldNotBeNull();
            model.Budget.ShouldNotBeNull();
            model.Budget.Start.ShouldEqual(new DateTime(2020, 1, 1));
            model.Budget.End.ShouldEqual(new DateTime(2020, 1, 31));
            model.Budget.Category_Budgets.Count.ShouldEqual(3);
            model.Accounts.ShouldNotBeNull();
            model.Accounts.Count().ShouldEqual(2);
            model.IncomeCategories.ShouldNotBeNull();
            model.IncomeCategories.Count().ShouldEqual(1);
            model.ExpenseCategories.ShouldNotBeNull();
            model.ExpenseCategories.Count().ShouldEqual(3);
            model.Transactions.ShouldNotBeNull();
            model.Transactions.Count.ShouldEqual(3);
            model.Account_AccountID.ShouldEqual(1);
        }
    }
}