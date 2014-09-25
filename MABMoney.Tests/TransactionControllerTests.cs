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
using MABMoney.Web.Models.Transactions;
using MABMoney.Data;
using System.Web.Mvc;
using MABMoney.Caching;

namespace MABMoney.Tests
{
    [TestFixture]
    public class TransactionControllerTests
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
        private ICachingHelpers _cachingHelpers;

        [SetUp]
        public void SetUp()
        {
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

            var accounts = new List<AccountDTO> {
                new AccountDTO { 
                    AccountID = 1,
                    Name = "Current",
                    StartingBalance = 100,
                    Categories = categories.Where(x => x.Account_AccountID == 1).ToList()
                }
            };

            var transactions = new List<TransactionDTO> {
                new TransactionDTO { 
                    TransactionID = 1,
                    Category_CategoryID = 1,
                    Account_AccountID = 1,
                    Date = new DateTime(2020, 1, 1),
                    Amount = -100.00M,
                    Description = "TEST1"
                },
                new TransactionDTO { 
                    TransactionID = 2,
                    Category_CategoryID = 2,
                    Account_AccountID = 1,
                    Date = new DateTime(2020, 1, 2),
                    Amount = 100.00M,
                    Description = "TEST2"
                },
                new TransactionDTO { 
                    TransactionID = 1,
                    Category_CategoryID = 3,
                    Account_AccountID = 1,
                    Date = new DateTime(2020, 1, 3),
                    Amount = -50.00M,
                    Description = null
                },
                new TransactionDTO { 
                    TransactionID = 4,
                    Category_CategoryID = 3,
                    Account_AccountID = 2,
                    Date = new DateTime(2020, 1, 4),
                    Amount = -75.00M,
                    Description = "NOT USED"
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
            _accountServices.Stub(x => x.GetForUser(1)).Return(accounts);

            _categoryServices = MockRepository.GenerateStub<ICategoryServices>();
            _categoryServices.Stub(x => x.All()).Return(categories);
            _categoryServices.Stub(x => x.Get(1)).Return(categories[0]);
            _categoryServices.Stub(x => x.Get(2)).Return(categories[1]);
            _categoryServices.Stub(x => x.Get(3)).Return(categories[2]);

            _transactionServices = MockRepository.GenerateStub<ITransactionServices>();
            _transactionServices.Stub(x => x.All()).Return(transactions);
            _transactionServices.Stub(x => x.Get(1)).Return(transactions[0]);
            _transactionServices.Stub(x => x.Get(2)).Return(transactions[1]);
            _transactionServices.Stub(x => x.Get(3)).Return(transactions[2]);
            _transactionServices.Stub(x => x.GetForAccount(1)).Return(transactions.Where(x => x.Account_AccountID == 1));
            _transactionServices.Stub(x => x.GetForAccount(1, DateTime.Now, DateTime.Now)).Return(transactions.Where(x => x.Account_AccountID == 1)).IgnoreArguments();

            _budgetServices = MockRepository.GenerateStub<IBudgetServices>();
            _context = MockRepository.GenerateStub<IHttpContextProvider>();

            _config = MockRepository.GenerateStub<ISiteConfiguration>();

            _config.Stub(x => x.Get<string>("SharedSecret")).Return("SHAREDSECRET");

            _dateProvider = MockRepository.GenerateStub<IDateTimeProvider>();
            _dateProvider.Stub(x => x.Now).Return(new DateTime(2020, 01, 01));

            _urlHelper = new FakeUrlHelper();

            _cache = MockRepository.GenerateStub<IModelCache>();

            _cachingHelpers = MockRepository.GenerateStub<ICachingHelpers>();
        }

        [Test]
        public void Index_Get()
        {
            var controller = new TransactionsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache, _cachingHelpers);

            var result = controller.Index(_profile, default(int?)) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as IndexViewModel;

            model.ShouldNotBeNull();
        }

        [Test]
        public void Index_Post()
        {
            var controller = new TransactionsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache, _cachingHelpers);

            var model = new IndexViewModel
            {
                AccountID = 1,
                From = DateTime.Now.AddDays(-30),
                To = DateTime.Now
            };

            var result = controller.Index(_profile, model) as ViewResult;

            result.ShouldNotBeNull();

            var resultModel = result.Model as IndexViewModel;

            resultModel.ShouldNotBeNull();
        }

        [Test]
        public void Create_Get()
        {
            var controller = new TransactionsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache, _cachingHelpers);

            var result = controller.Create(_profile) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as CreateViewModel;

            model.ShouldNotBeNull();
            model.Account_AccountID.ShouldEqual(0);
            model.Category_CategoryID.ShouldEqual(null);
            model.Date.ShouldEqual(new DateTime(2020, 1, 1));
            model.Amount.ShouldEqual(0);
            model.Description.ShouldEqual(null);
        }

        [Test]
        public void Create_Post()
        {
            var controller = new TransactionsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache, _cachingHelpers);

            var model = new CreateViewModel
            {
                Account_AccountID = 1,
                Category_CategoryID = 1,
                Date = new DateTime(2020, 12, 31),
                Amount = -80M,
                Description = "NEW TRANSACTION",
                RedirectAfterSubmitUrl = "http://localhost"
            };

            var result = controller.Create(_profile, model) as RedirectResult;

            result.ShouldNotBeNull();

            _transactionServices.AssertWasCalled(x => x.Save(
                Arg<TransactionDTO>.Matches(
                    o => o.Account_AccountID == 1
                      && o.Category_CategoryID == 1
                      && o.Date == new DateTime(2020, 12, 31)
                      && o.Amount == -80M
                      && o.Description == "NEW TRANSACTION"
               )
            ));
        }

        [Test]
        public void Edit_Get()
        {
            var controller = new TransactionsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache, _cachingHelpers);

            var result = controller.Edit(_profile, 2) as ViewResult;

            result.ShouldNotBeNull();

            var model = result.Model as EditViewModel;

            model.ShouldNotBeNull();
            model.TransactionID.ShouldEqual(2);
            model.Account_AccountID.ShouldEqual(1);
            model.Category_CategoryID.ShouldEqual(2);
            model.Date.ShouldEqual(new DateTime(2020, 1, 2));
            model.Amount.ShouldEqual(100M);
            model.Description.ShouldEqual("TEST2");
        }

        [Test]
        public void Edit_Post()
        {
            var controller = new TransactionsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache, _cachingHelpers);

            var model = new EditViewModel
            {
                TransactionID = 3,
                Account_AccountID = 1,
                Category_CategoryID = 3,
                Date = new DateTime(2020, 12, 30),
                Amount = -65M,
                Description = "EDITED",
                RedirectAfterSubmitUrl = "http://localhost"
            };

            var result = controller.Edit(_profile, model) as RedirectResult;

            result.ShouldNotBeNull();

            _transactionServices.AssertWasCalled(x => x.Save(
                Arg<TransactionDTO>.Matches(
                    o => o.TransactionID == 3
                      && o.Account_AccountID == 1
                      && o.Category_CategoryID == 3
                      && o.Date == new DateTime(2020, 12, 30)
                      && o.Amount == -65M
                      && o.Description == "EDITED"
               )
            ));
        }

        [Test]
        public void Delete_Post()
        {
            var controller = new TransactionsController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider, _urlHelper, _cache, _cachingHelpers);

            var result = controller.Delete(_profile, 3, "/Transactions") as RedirectResult;

            result.ShouldNotBeNull();

            _transactionServices.AssertWasCalled(x => x.Delete(
                Arg<int>.Is.Equal(3)
            ));
        }
    }
}
