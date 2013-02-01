using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MABMoney.Services;
using System.Web;
using MABMoney.Web.Infrastructure;
using MABMoney.Web.Models;
using MABMoney.Web.Models.Budgets;
using MABMoney.Services.DTO;
using Rhino.Mocks;
using MABMoney.Web.Controllers;
using System.Web.Mvc;

namespace MABMoney.Tests
{
    [TestFixture]
    public class BudgetControllerTests
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
        private ICacheProvider _cacheProvider;

        [SetUp]
        public void SetUp()
        {
            _cacheProvider = MockRepository.GenerateMock<ICacheProvider>();
            _cacheProvider.Stub(x => x.Get<object>(null)).IgnoreArguments().Return(null);

            var accounts = new List<AccountDTO> {
                new AccountDTO { 
                    AccountID = 1,
                    Name = "Current",
                    StartingBalance = 100
                },
                new AccountDTO { 
                    AccountID = 2,
                    Name = "Savings",
                    StartingBalance = 200
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
                            Category = categories[1],
                            Budget_BudgetID = 1,
                            Category_CategoryID = 2,
                            Amount = 1000,
                            Total = 1000
                        },
                        new Category_BudgetDTO {
                            Category = categories[2],
                            Budget_BudgetID = 1,
                            Category_CategoryID = 3,
                            Amount = 250,
                            Total = 50
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
                            Total = 100
                        },
                        new Category_BudgetDTO {
                            Category = new CategoryDTO {
                                Name = "Uncategorised",
                                Type = CategoryTypeDTO.Expense,
                                CategoryID = 0
                            },
                            Category_CategoryID = 0,
                            Amount = 100,
                            Total = 0
                        }
                    }
                }
            };

            _profile = new ProfileViewModel
            {
                UserID = 1
            };

            _userServices = MockRepository.GenerateStub<IUserServices>();

            _accountServices = MockRepository.GenerateStub<IAccountServices>();
            _accountServices.Stub(x => x.All()).Return(accounts);
            _accountServices.Stub(x => x.Get(1)).Return(accounts[0]);
            _accountServices.Stub(x => x.Get(2)).Return(accounts[1]);

            _categoryServices = MockRepository.GenerateStub<ICategoryServices>();
            _categoryServices.Stub(x => x.All()).Return(categories);
            _categoryServices.Stub(x => x.Get(1)).Return(categories[0]);
            _categoryServices.Stub(x => x.Get(2)).Return(categories[1]);

            _transactionServices = MockRepository.GenerateStub<ITransactionServices>();

            _budgetServices = MockRepository.GenerateStub<IBudgetServices>();
            _budgetServices.Stub(x => x.All()).Return(budgets);
            _budgetServices.Stub(x => x.Get(1)).Return(budgets[0]);
            _budgetServices.Stub(x => x.Get(2)).Return(budgets[1]);

            _context = MockRepository.GenerateStub<HttpContextBase>();

            _config = MockRepository.GenerateStub<ISiteConfiguration>();

            _config.Stub(x => x.SharedSecret).Return("SHAREDSECRET");

            _dateProvider = MockRepository.GenerateStub<IDateTimeProvider>();
            _dateProvider.Stub(x => x.Date).Return(new DateTime(2020, 01, 01));
        }

        [Test]
        public void Index_Get()
        {
            var controller = new BudgetsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _cacheProvider);

            var result = controller.Index(_profile) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as IndexViewModel;

            model.ShouldNotBeNull();
            model.Budgets.Count.ShouldEqual(2);
            model.Budgets[0].BudgetID.ShouldEqual(1);
            model.Budgets[0].Start.ShouldEqual(new DateTime(2020, 1, 1));
            model.Budgets[0].End.ShouldEqual(new DateTime(2020, 1, 31));
            model.Budgets[1].BudgetID.ShouldEqual(2);
            model.Budgets[1].Start.ShouldEqual(new DateTime(2021, 1, 1));
            model.Budgets[1].End.ShouldEqual(new DateTime(2021, 1, 31));
        }

        [Test]
        public void Create_Get()
        {
            var controller = new BudgetsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _cacheProvider);

            var result = controller.Create(_profile, 1) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as CreateViewModel;

            model.ShouldNotBeNull();
        }

        [Test]
        public void Create_Post()
        {
            var controller = new BudgetsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _cacheProvider);

            var model = new CreateViewModel
            {
                Account_AccountID = 2,
                Start = new DateTime(2020, 3, 1),
                End = new DateTime(2020, 3, 31),
                Categories = new List<Category_BudgetViewModel> { 
                    new Category_BudgetViewModel {
                        Category_CategoryID = 4,
                        Amount = 10
                    }
                }
            };

            var result = controller.Create(_profile, model) as RedirectToRouteResult;

            result.ShouldNotBeNull();

            _budgetServices.AssertWasCalled(x => x.Save(
                Arg<BudgetDTO>.Matches(
                    o => o.Start == new DateTime(2020, 3, 1)
                      && o.End == new DateTime(2020, 3, 31)
                      && o.Account_AccountID == 2
                )
            ));

            _budgetServices.AssertWasCalled(x => x.SaveCategoryBudget(
                Arg<Category_BudgetDTO>.Matches(
                    o => o.Category_CategoryID == 4
                      && o.Amount == 10
                )
            ));
        }

        [Test]
        public void Edit_Get()
        {
            var controller = new BudgetsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _cacheProvider);

            var result = controller.Edit(_profile, 2) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as EditViewModel;

            model.ShouldNotBeNull();
            model.BudgetID.ShouldEqual(2);
            model.Start.ShouldEqual(new DateTime(2021, 1, 1));
            model.End.ShouldEqual(new DateTime(2021, 1, 31));
            model.Account_AccountID.ShouldEqual(2);
            model.Categories.Count.ShouldEqual(1);
            model.Categories[0].Name.ShouldEqual("Deposit");
            model.Categories[0].Amount.ShouldEqual(50M);
            model.Categories[0].Total.ShouldEqual(100M);
            model.Categories[0].Category_CategoryID.ShouldEqual(4);
            model.Categories[0].Budget_BudgetID.ShouldEqual(2);
        }

        [Test]
        public void Edit_Post()
        {
            var controller = new BudgetsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _cacheProvider);

            var model = new EditViewModel
            {
                BudgetID = 10,
                Account_AccountID = 1,
                Start = new DateTime(2020, 12, 1),
                End = new DateTime(2020, 12, 31),
                Categories = new List<Category_BudgetViewModel> { 
                    new Category_BudgetViewModel {
                        Category_CategoryID = 3,
                        Amount = 70
                    }
                }
            };

            var result = controller.Edit(_profile, model) as RedirectToRouteResult;

            result.ShouldNotBeNull();

            _budgetServices.AssertWasCalled(x => x.Save(
               Arg<BudgetDTO>.Matches(
                   o => o.BudgetID == 10
                     && o.Start == new DateTime(2020, 12, 1)
                     && o.End == new DateTime(2020, 12, 31)
                     && o.Account_AccountID == 1
               )
           ));

            _budgetServices.AssertWasCalled(x => x.SaveCategoryBudget(
                Arg<Category_BudgetDTO>.Matches(
                    o => o.Category_CategoryID == 3
                      && o.Amount == 70
                )
            ));
        }

        [Test]
        public void Delete_Post()
        {
            var controller = new BudgetsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _cacheProvider);

            var result = controller.Delete(_profile, 2) as RedirectToRouteResult;

            result.ShouldNotBeNull();

            _budgetServices.AssertWasCalled(x => x.Delete(
                Arg<int>.Is.Equal(2)
            ));
        }
    }
}
