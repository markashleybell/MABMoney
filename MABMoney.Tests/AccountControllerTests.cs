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
                    StartingBalance = 100,
                    User_UserID = 1
                },
                new AccountDTO { 
                    AccountID = 2,
                    Name = "Savings",
                    StartingBalance = 200,
                    User_UserID = 1
                },
                new AccountDTO { 
                    AccountID = 3,
                    Name = "Other",
                    StartingBalance = 150,
                    User_UserID = 2
                }
            };

            _profile = new ProfileViewModel { 
                UserID = 1
            };

            _userServices = MockRepository.GenerateStub<IUserServices>();

            _accountServices = MockRepository.GenerateStub<IAccountServices>();
            _accountServices.Stub(x => x.All(1)).Return(accounts.Where(x => x.User_UserID == 1));
            _accountServices.Stub(x => x.Get(1, 1)).Return(accounts[0]);

            _categoryServices = MockRepository.GenerateStub<ICategoryServices>();
            _transactionServices = MockRepository.GenerateStub<ITransactionServices>();
            _budgetServices = MockRepository.GenerateStub<IBudgetServices>();
            _context = MockRepository.GenerateStub<HttpContextBase>();

            _config = MockRepository.GenerateStub<ISiteConfiguration>();

            _config.Stub(x => x.SharedSecret).Return("SHAREDSECRET");

            _dateProvider = MockRepository.GenerateStub<IDateTimeProvider>();
            _dateProvider.Stub(x => x.Date).Return(new DateTime(2020, 01, 01));
        }

        [Test]
        public void Index_Get()
        {
            var controller = new AccountsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

            var result = controller.Index(_profile) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as IndexViewModel;

            model.ShouldNotBeNull();
            model.Accounts.Count.ShouldEqual(2);
            model.Accounts[0].AccountID.ShouldEqual(1);
            model.Accounts[0].Name.ShouldEqual("Current");
            model.Accounts[0].StartingBalance.ShouldEqual(100M);
            model.Accounts[0].User_UserID.ShouldEqual(1);
            model.Accounts[1].AccountID.ShouldEqual(2);
            model.Accounts[1].Name.ShouldEqual("Savings");
            model.Accounts[1].StartingBalance.ShouldEqual(200M);
            model.Accounts[1].User_UserID.ShouldEqual(1);
        }

        [Test]
        public void Create_Get()
        {
            var controller = new AccountsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

            var result = controller.Create(_profile) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as CreateViewModel;

            model.ShouldNotBeNull();
            model.User_UserID.ShouldEqual(1);
        }

        [Test]
        public void Create_Post()
        {
            var controller = new AccountsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

            var model = new CreateViewModel
            {
                User_UserID = 1,
                Name = "New",
                StartingBalance = 1000M
            };

            var result = controller.Create(_profile, model) as RedirectToRouteResult;

            result.ShouldNotBeNull();

            _accountServices.AssertWasCalled(x => x.Save(
                Arg<int>.Is.Equal(1), 
                Arg<AccountDTO>.Matches(o => o.User_UserID == 1 && o.Name == "New" && o.StartingBalance == 1000M)
            ));
        }

        [Test]
        public void Edit_Get()
        {
            var controller = new AccountsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

            var result = controller.Edit(_profile, 1) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as EditViewModel;

            model.ShouldNotBeNull();
            model.User_UserID.ShouldEqual(1);
            model.AccountID.ShouldEqual(1);
            model.StartingBalance.ShouldEqual(100M);
            model.Name.ShouldEqual("Current");
        }

        [Test]
        public void Edit_Post()
        {
            var controller = new AccountsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

            var model = new EditViewModel
            {
                User_UserID = 1,
                AccountID = 10,
                Name = "EDITED",
                StartingBalance = 750M
            };

            var result = controller.Edit(_profile, model) as RedirectToRouteResult;

            result.ShouldNotBeNull();

            _accountServices.AssertWasCalled(x => x.Save(
                Arg<int>.Is.Equal(1),
                Arg<AccountDTO>.Matches(o => o.User_UserID == 1 && o.AccountID == 10 && o.Name == "EDITED" && o.StartingBalance == 750M)
            ));
        }

        [Test]
        public void Delete_Post()
        {
            var controller = new AccountsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

            var result = controller.Delete(_profile, 2) as RedirectToRouteResult;

            result.ShouldNotBeNull();

            _accountServices.AssertWasCalled(x => x.Delete(
                Arg<int>.Is.Equal(1),
                Arg<int>.Is.Equal(2)
            ));
        }
    }
}
