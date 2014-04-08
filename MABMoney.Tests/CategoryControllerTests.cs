using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MABMoney.Services;
using System.Web;
using MABMoney.Web.Infrastructure;
using MABMoney.Web.Models;
using MABMoney.Services.DTO;
using Rhino.Mocks;
using MABMoney.Web.Controllers;
using System.Web.Mvc;
using MABMoney.Web.Models.Categories;
using MABMoney.Data;
using MABMoney.Caching;

namespace MABMoney.Tests
{
    [TestFixture]
    public class CategoryControllerTests
    {
        private IUserServices _userServices;
        private IAccountServices _accountServices;
        private ICategoryServices _categoryServices;
        private ITransactionServices _transactionServices;
        private IBudgetServices _budgetServices;
        private IHttpContextProvider _context;
        private ISiteConfiguration _config;
        private ProfileViewModel _profile;
        private IDateTimeProvider _dateProvider;
        private IUrlHelper _urlHelper;
        private IModelCache _cache;

        [SetUp]
        public void SetUp()
        {
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
                },
                new AccountDTO { 
                    AccountID = 3,
                    Name = "Other",
                    StartingBalance = 150
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
            _accountServices.Stub(x => x.Get(3)).Return(accounts[2]);

            _categoryServices = MockRepository.GenerateStub<ICategoryServices>();
            _categoryServices.Stub(x => x.All()).Return(categories);
            _categoryServices.Stub(x => x.Get(1)).Return(categories[0]);
            _categoryServices.Stub(x => x.Get(2)).Return(categories[1]);
            _categoryServices.Stub(x => x.Get(3)).Return(categories[2]);

            _transactionServices = MockRepository.GenerateStub<ITransactionServices>();
            _budgetServices = MockRepository.GenerateStub<IBudgetServices>();
            _context = MockRepository.GenerateStub<IHttpContextProvider>();

            _config = MockRepository.GenerateStub<ISiteConfiguration>();

            _config.Stub(x => x.Get<string>("SharedSecret")).Return("SHAREDSECRET");

            _dateProvider = MockRepository.GenerateStub<IDateTimeProvider>();
            _dateProvider.Stub(x => x.Now).Return(new DateTime(2020, 01, 01));

            _urlHelper = new FakeUrlHelper();

            _cache = MockRepository.GenerateStub<IModelCache>();
        }

        [Test]
        public void Index_Get()
        {
            var controller = new CategoriesController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache);

            var result = controller.Index(_profile) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as IndexViewModel;

            model.ShouldNotBeNull();
            model.Categories.Count.ShouldEqual(3);
            model.Categories[0].CategoryID.ShouldEqual(1);
            model.Categories[1].CategoryID.ShouldEqual(2);
            model.Categories[2].CategoryID.ShouldEqual(3);
        }

        [Test]
        public void Create_Get()
        {
            var controller = new CategoriesController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache);

            var result = controller.Create(_profile) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as CreateViewModel;

            model.ShouldNotBeNull();
            model.Name.ShouldEqual(null);
            model.Account_AccountID.ShouldEqual(0);
            model.Type.ShouldEqual(CategoryTypeDTO.Expense);
        }

        [Test]
        public void Create_Post()
        {
            var controller = new CategoriesController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache);

            var model = new CreateViewModel
            {
                Account_AccountID = 2,
                Name = "New",
                Type = CategoryTypeDTO.Income,
                RedirectAfterSubmitUrl = "http://localhost"
            };

            var result = controller.Create(_profile, model) as RedirectResult;

            result.ShouldNotBeNull();

            _categoryServices.AssertWasCalled(x => x.Save(
                Arg<CategoryDTO>.Matches(o => o.Account_AccountID == 2 && o.Name == "New" && o.Type == CategoryTypeDTO.Income)
            ));
        }

        [Test]
        public void Edit_Get()
        {
            var controller = new CategoriesController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache);

            var result = controller.Edit(_profile, 2) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as EditViewModel;

            model.ShouldNotBeNull();
            model.CategoryID.ShouldEqual(2);
            model.Account_AccountID.ShouldEqual(1);
            model.Type.ShouldEqual(CategoryTypeDTO.Income);
            model.Name.ShouldEqual("Salary");
        }

        [Test]
        public void Edit_Post()
        {
            var controller = new CategoriesController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache);

            var model = new EditViewModel
            {
                CategoryID = 10,
                Account_AccountID = 2,
                Name = "EDITED",
                Type = CategoryTypeDTO.Expense,
                RedirectAfterSubmitUrl = "http://localhost"
            };

            var result = controller.Edit(_profile, model) as RedirectResult;

            result.ShouldNotBeNull();

            _categoryServices.AssertWasCalled(x => x.Save(
                Arg<CategoryDTO>.Matches(o => o.CategoryID == 10 && o.Account_AccountID == 2 && o.Name == "EDITED" && o.Type == CategoryTypeDTO.Expense)
            ));
        }

        [Test]
        public void Delete_Post()
        {
            var controller = new CategoriesController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache);

            var result = controller.Delete(_profile, 2, "/Categories") as RedirectResult;

            result.ShouldNotBeNull();

            _categoryServices.AssertWasCalled(x => x.Delete(
                Arg<int>.Is.Equal(2)
            ));
        }
    }
}
