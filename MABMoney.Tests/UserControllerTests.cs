using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using MABMoney.Services;
using MABMoney.Services.DTO;
using System.Web;
using MABMoney.Web.Controllers;
using MABMoney.Web.Infrastructure;
using System.Web.Mvc;
using MABMoney.Web.Models.Users;
using MABMoney.Data;

namespace MABMoney.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private IUserServices _userServices;
        private IAccountServices _accountServices;
        private ICategoryServices _categoryServices;
        private ITransactionServices _transactionServices;
        private IBudgetServices _budgetServices;
        private FakeHttpContextProvider _context;
        private ICryptoProvider _crypto;
        private ISiteConfiguration _config;
        private IDateTimeProvider _dateProvider;
        private IUrlHelper _urlHelper;

        [SetUp]
        public void SetUp()
        {
            var users = new List<UserDTO> {
                new UserDTO { 
                    UserID = 1,
                    Forename = "Bob",
                    Surname = "Jones",
                    Email = "bob@bob.com",
                    Password = "xxxxx",
                    IsAdmin = true
                },
                new UserDTO { 
                    UserID = 2,
                    Forename = "Jane",
                    Surname = "Smith",
                    Email = "jane@jane.com",
                    Password = "yyyyy",
                    IsAdmin = false
                }
            };

            _userServices = MockRepository.GenerateStub<IUserServices>();
            _userServices.Stub(x => x.All()).Return(users);
            _userServices.Stub(x => x.Get(1)).Return(users[0]);
            _userServices.Stub(x => x.GetByEmailAddress("jane@jane.com")).Return(users[1]);

            _accountServices = MockRepository.GenerateStub<IAccountServices>();
            _categoryServices = MockRepository.GenerateStub<ICategoryServices>();
            _transactionServices = MockRepository.GenerateStub<ITransactionServices>();
            _budgetServices = MockRepository.GenerateStub<IBudgetServices>();
            _context = new FakeHttpContextProvider();

            _crypto = MockRepository.GenerateStub<ICryptoProvider>();

            _crypto.Stub(x => x.VerifyHashedPassword("yyyyy", "test123")).Return(true);

            _config = MockRepository.GenerateStub<ISiteConfiguration>();

            _config.Stub(x => x.SharedSecret).Return("SHAREDSECRET");
            _config.Stub(x => x.CookieKey).Return("COOKIEKEY");

            _dateProvider = MockRepository.GenerateStub<IDateTimeProvider>();
            _dateProvider.Stub(x => x.Now).Return(new DateTime(2020, 01, 01));

            _urlHelper = new FakeUrlHelper();
        }    

        [Test]
        public void Index_Get()
        {
            var controller = new UsersController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _crypto, _urlHelper);

            var result = controller.Index() as ViewResult;
            
            Assert.NotNull(result);

            var model = result.Model as IndexViewModel;

            Assert.NotNull(model);
            Assert.AreEqual(2, model.Users.Count);
            Assert.AreEqual(1, model.Users[0].UserID);
            Assert.AreEqual("Bob", model.Users[0].Forename);
            Assert.AreEqual("Jones", model.Users[0].Surname);
            Assert.AreEqual("bob@bob.com", model.Users[0].Email);
            Assert.AreEqual("xxxxx", model.Users[0].Password);
            Assert.AreEqual(2, model.Users[1].UserID);
            Assert.AreEqual("Jane", model.Users[1].Forename);
            Assert.AreEqual("Smith", model.Users[1].Surname);
            Assert.AreEqual("jane@jane.com", model.Users[1].Email);
            Assert.AreEqual("yyyyy", model.Users[1].Password);
        }

        [Test]
        public void Create_Get()
        {
            var controller = new UsersController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _crypto, _urlHelper);

            var result = controller.Create() as ViewResult;

            Assert.NotNull(result);

            var model = result.Model as CreateViewModel;

            Assert.NotNull(model);
            Assert.AreEqual(null, model.Forename);
            Assert.AreEqual(null, model.Surname);
            Assert.AreEqual(null, model.Email);
        }

        [Test]
        public void Create_Post()
        {
            var controller = new UsersController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _crypto, _urlHelper);

            var model = new CreateViewModel {
                Forename = "Test",
                Surname = "User",
                Email = "test@test.com",
                RedirectAfterSubmitUrl = "/"
            };

            var result = controller.Create(model) as RedirectResult;
            
            _userServices.AssertWasCalled(x => x.Save(Arg<UserDTO>.Matches(o => o.Forename == "Test" && o.Surname == "User" && o.Email == "test@test.com")));

            Assert.NotNull(result);
        }

        [Test]
        public void Edit_Get()
        {
            var controller = new UsersController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _crypto, _urlHelper);

            var result = controller.Edit(1) as ViewResult;

            Assert.NotNull(result);

            var model = result.Model as EditViewModel;

            Assert.NotNull(model);
            Assert.AreEqual("Bob", model.Forename);
            Assert.AreEqual("Jones", model.Surname);
            Assert.AreEqual("bob@bob.com", model.Email);
        }

        [Test]
        public void Edit_Post()
        {
            var controller = new UsersController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _crypto, _urlHelper);

            var model = new EditViewModel {
                UserID = 1,
                Forename = "Test",
                Surname = "User",
                Email = "test@test.com",
                RedirectAfterSubmitUrl = "/"
            };

            var result = controller.Edit(model) as RedirectResult;
            
            _userServices.AssertWasCalled(x => x.Save(Arg<UserDTO>.Matches(o => o.Forename == "Test" && o.Surname == "User" && o.Email == "test@test.com")));

            Assert.NotNull(result);
        }

        [Test]
        public void Delete_Post()
        {
            var controller = new UsersController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _crypto, _urlHelper);

            var result = controller.Delete(2, "/Users") as RedirectResult;
            
            _userServices.AssertWasCalled(x => x.Delete(Arg<int>.Is.Equal(2)));

            Assert.NotNull(result);
        }

        [Test]
        public void Login_Get()
        {
            var controller = new UsersController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _crypto, _urlHelper);

            var result = controller.Login() as ViewResult;

            Assert.NotNull(result);

            var model = result.Model as LoginViewModel;

            Assert.NotNull(model);
            Assert.AreEqual(null, model.Email);
            Assert.AreEqual(null, model.Password);
        }

        [Test]
        public void Login_Post()
        {
            var controller = new UsersController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _crypto, _urlHelper);

            var model = new LoginViewModel {
                Email = "jane@jane.com",
                Password = "test123",
                RedirectAfterSubmitUrl = "/"
            };

            var result = controller.Login(model) as RedirectResult;
            
            _userServices.AssertWasCalled(x => x.GetByEmailAddress(model.Email));

            _crypto.AssertWasCalled(x => x.VerifyHashedPassword("yyyyy", model.Password));

            Assert.AreEqual("COOKIEKEY", _context.Cookies.First().Key);

            Assert.NotNull(result);
        }

        [Test]
        public void Logout_Get()
        {
            var controller = new UsersController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _crypto, _urlHelper);

            var result = controller.Logout() as RedirectToRouteResult;

            Assert.AreEqual("COOKIEKEY", _context.Cookies.First().Key);
            Assert.Greater(_dateProvider.Now, _context.Cookies.First().Value.Item2);

            Assert.NotNull(result);
        }
    }
}
