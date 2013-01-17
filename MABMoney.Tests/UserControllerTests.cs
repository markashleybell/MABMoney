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
        private HttpContextBase _context;
        private ICryptoWrapper _crypto;

        [SetUp]
        public void SetUp()
        {
            _userServices = MockRepository.GenerateStub<IUserServices>();

            _userServices.Stub(x => x.All()).Return(new List<UserDTO> {
                new UserDTO { 
                    UserID = 1,
                    Forename = "Bob",
                    Surname = "Jones",
                    Email = "bob@bob.com",
                    Password = "xxxxx"
                },
                new UserDTO { 
                    UserID = 2,
                    Forename = "Jane",
                    Surname = "Smith",
                    Email = "jane@jane.com",
                    Password = "yyyyy"
                }
            });

            _accountServices = MockRepository.GenerateStub<IAccountServices>();
            _categoryServices = MockRepository.GenerateStub<ICategoryServices>();
            _transactionServices = MockRepository.GenerateStub<ITransactionServices>();
            _budgetServices = MockRepository.GenerateStub<IBudgetServices>();
            _context = MockRepository.GenerateStub<HttpContextBase>();
            _crypto = MockRepository.GenerateStub<ICryptoWrapper>();
        }    

        [Test]
        public void Index()
        {
            var controller = new UsersController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, null);

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
    }
}
