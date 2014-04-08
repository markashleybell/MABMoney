using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MABMoney.Services;
using System.Web;
using MABMoney.Web.Infrastructure;
using Rhino.Mocks;
using MABMoney.Services.DTO;
using MABMoney.Web.Controllers;
using System.Web.Mvc;
using MABMoney.Web.Models;
using MABMoney.Web.Models.Accounts;
using MABMoney.Data;
using MABMoney.Caching;

namespace MABMoney.Tests
{
    [TestFixture]
    public class AccountControllerTests
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
                    StartingBalance = 100,
                    Type = AccountTypeDTO.Current,
                    DisplayOrder = 0
                },
                new AccountDTO { 
                    AccountID = 2,
                    Name = "Savings",
                    StartingBalance = 200,
                    Type = AccountTypeDTO.Savings,
                    DisplayOrder = 1
                },
                new AccountDTO { 
                    AccountID = 3,
                    Name = "Other",
                    StartingBalance = 150,
                    Type = AccountTypeDTO.CreditCard,
                    DisplayOrder = 2
                }
            };

            _profile = new ProfileViewModel { 
                UserID = 1
            };

            _userServices = MockRepository.GenerateStub<IUserServices>();

            _accountServices = MockRepository.GenerateStub<IAccountServices>();
            _accountServices.Stub(x => x.All()).Return(accounts);
            _accountServices.Stub(x => x.Get(1)).Return(accounts[0]);

            _categoryServices = MockRepository.GenerateStub<ICategoryServices>();
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
            var controller = new AccountsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache);

            var result = controller.Index(_profile) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as IndexViewModel;

            model.ShouldNotBeNull();
            model.Accounts.Count.ShouldEqual(3);
            model.Accounts[0].AccountID.ShouldEqual(1);
            model.Accounts[0].Name.ShouldEqual("Current");
            model.Accounts[0].StartingBalance.ShouldEqual(100M);
            model.Accounts[0].Type.ShouldEqual(AccountTypeDTO.Current);
            model.Accounts[1].AccountID.ShouldEqual(2);
            model.Accounts[1].Name.ShouldEqual("Savings");
            model.Accounts[1].StartingBalance.ShouldEqual(200M);
            model.Accounts[1].Type.ShouldEqual(AccountTypeDTO.Savings);
        }

        [Test]
        public void Create_Get()
        {
            var controller = new AccountsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache);

            var result = controller.Create(_profile) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as CreateViewModel;

            model.ShouldNotBeNull();
            model.Type.ShouldEqual(AccountTypeDTO.Current);
        }

        [Test]
        public void Create_Post()
        {
            var controller = new AccountsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache);

            var model = new CreateViewModel
            {
                Name = "New",
                StartingBalance = 1000M,
                Type = AccountTypeDTO.Current,
                RedirectAfterSubmitUrl = "http://localhost",
                DisplayOrder = 4
            };

            var result = controller.Create(_profile, model) as RedirectToRouteResult;

            result.ShouldNotBeNull();

            _accountServices.AssertWasCalled(x => x.Save(
                Arg<AccountDTO>.Matches(
                    o => o.Name == "New" 
                      && o.StartingBalance == 1000M
                      && o.Type == AccountTypeDTO.Current
                      && o.DisplayOrder == 4
                )
            ));
        }

        [Test]
        public void Edit_Get()
        {
            var controller = new AccountsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache);

            var result = controller.Edit(_profile, 1) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as EditViewModel;

            model.ShouldNotBeNull();
            model.AccountID.ShouldEqual(1);
            model.StartingBalance.ShouldEqual(100M);
            model.Name.ShouldEqual("Current");
            model.Type.ShouldEqual(AccountTypeDTO.Current);
            model.DisplayOrder.ShouldEqual(0);
        }

        [Test]
        public void Edit_Post()
        {
            var controller = new AccountsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache);

            var model = new EditViewModel
            {
                AccountID = 10,
                Name = "EDITED",
                StartingBalance = 750M,
                Type = AccountTypeDTO.CreditCard,
                RedirectAfterSubmitUrl = "http://localhost",
                DisplayOrder = 5
            };

            var result = controller.Edit(_profile, model) as RedirectResult;

            result.ShouldNotBeNull();

            _accountServices.AssertWasCalled(x => x.Save(
                Arg<AccountDTO>.Matches(
                    o => o.AccountID == 10 
                      && o.Name == "EDITED" 
                      && o.StartingBalance == 750M
                      && o.Type == AccountTypeDTO.CreditCard
                      && o.DisplayOrder == 5
                )
            ));
        }

        [Test]
        public void Delete_Post()
        {
            var controller = new AccountsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache);

            var result = controller.Delete(_profile, 2, "/Accounts") as RedirectResult;

            result.ShouldNotBeNull();

            _accountServices.AssertWasCalled(x => x.Delete(
                Arg<int>.Is.Equal(2)
            ));
        }
    }
}
