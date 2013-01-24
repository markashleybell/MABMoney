﻿using System;
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
        private HttpContextBase _context;
        private ISiteConfiguration _config;
        private HttpCookieCollection _cookies;
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
            _accountServices.Stub(x => x.All(1)).Return(accounts.Where(x => x.User_UserID == 1));
            _accountServices.Stub(x => x.Get(1, 1)).Return(accounts[0]);
            _accountServices.Stub(x => x.Get(1, 2)).Return(accounts[1]);
            _accountServices.Stub(x => x.Get(1, 3)).Return(accounts[2]);

            _categoryServices = MockRepository.GenerateStub<ICategoryServices>();
            _categoryServices.Stub(x => x.All(1)).Return(categories);
            _categoryServices.Stub(x => x.Get(1, 1)).Return(categories[0]);
            _categoryServices.Stub(x => x.Get(1, 2)).Return(categories[1]);
            _categoryServices.Stub(x => x.Get(1, 3)).Return(categories[2]);

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
            var controller = new CategoriesController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

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
            var controller = new CategoriesController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

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
            var controller = new CategoriesController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

            var model = new CreateViewModel
            {
                Account_AccountID = 2,
                Name = "New",
                Type = CategoryTypeDTO.Income
            };

            var result = controller.Create(_profile, model) as RedirectToRouteResult;

            result.ShouldNotBeNull();

            _categoryServices.AssertWasCalled(x => x.Save(
                Arg<int>.Is.Equal(1),
                Arg<CategoryDTO>.Matches(o => o.Account_AccountID == 2 && o.Name == "New" && o.Type == CategoryTypeDTO.Income)
            ));
        }

        [Test]
        public void Edit_Get()
        {
            var controller = new CategoriesController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

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
            var controller = new CategoriesController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

            var model = new EditViewModel
            {
                CategoryID = 10,
                Account_AccountID = 2,
                Name = "EDITED",
                Type = CategoryTypeDTO.Expense
            };

            var result = controller.Edit(_profile, model) as RedirectToRouteResult;

            result.ShouldNotBeNull();

            _categoryServices.AssertWasCalled(x => x.Save(
                Arg<int>.Is.Equal(1),
                Arg<CategoryDTO>.Matches(o => o.CategoryID == 10 && o.Account_AccountID == 2 && o.Name == "EDITED" && o.Type == CategoryTypeDTO.Expense)
            ));
        }

        [Test]
        public void Delete_Post()
        {
            var controller = new CategoriesController(_userServices, _accountServices, _categoryServices, _transactionServices, _budgetServices, _context, _config, _dateProvider);

            var result = controller.Delete(_profile, 2) as RedirectToRouteResult;

            result.ShouldNotBeNull();

            _categoryServices.AssertWasCalled(x => x.Delete(
                Arg<int>.Is.Equal(1),
                Arg<int>.Is.Equal(2)
            ));
        }
    }
}
