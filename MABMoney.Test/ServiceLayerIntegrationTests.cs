using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using MABMoney.Data.Concrete;
using MABMoney.Data.Abstract;
using System.IO;
using MABMoney.Services;
using MABMoney.Services.DTO;
using System.Data;
using MABMoney.Domain;

namespace MABMoney.Test
{
    [TestFixture]
    public class ServiceLayerIntegrationTests
    {
        private string _setupConnectionString;
        private string _dataConnectionString;
        private string _sqlScriptSource;

        private IAccountRepository _accountRepository;
        private IBudgetRepository _budgetRepository;
        private ICategory_BudgetRepository _category_budgetRepository;
        private ICategoryRepository _categoryRepository;
        private ISessionRepository _sessionRepository;
        private ITransactionRepository _transactionRepository;
        private IUserRepository _userRepository;

        private void DeleteTestDatabase(string connectionString)
        {
            // Delete the test database
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var serverConnection = new ServerConnection(conn);
                var server = new Server(serverConnection);

                var exists = conn.ExecuteScalar("SELECT name FROM master.dbo.sysdatabases WHERE name = N'MABMoney_TEST'");

                if (exists != null)
                {
                    server.KillDatabase("MABMoney_TEST");
                }
            }
        }

        private T GetSingle<T>(string sql)
        {
            using (var connection = new SqlConnection(_dataConnectionString))
            {
                return connection.Query<T>(sql).SingleOrDefault();
            }
        }

        private T GetScalar<T>(string sql)
        {
            using (var connection = new SqlConnection(_dataConnectionString))
            {
                return connection.ExecuteScalar<T>(sql);
            }
        }

        private IEnumerable<T> GetEnumerable<T>(string sql)
        {
            using (var connection = new SqlConnection(_dataConnectionString))
            {
                return connection.Query<T>(sql);
            }
        }

        #region SetUp

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _setupConnectionString = ConfigurationManager.AppSettings["SetUpDbConnectionString"];
            _dataConnectionString = ConfigurationManager.AppSettings["DataDbConnectionString"];
            _sqlScriptSource = ConfigurationManager.AppSettings["SqlScriptSource"];

            DeleteTestDatabase(_setupConnectionString);

            // TODO: Make sure SQL build scripts have been run

            _accountRepository = new AccountRepository(_dataConnectionString, 1);
            _budgetRepository = new BudgetRepository(_dataConnectionString, 1);
            _category_budgetRepository = new Category_BudgetRepository(_dataConnectionString, 1);
            _categoryRepository = new CategoryRepository(_dataConnectionString, 1);
            _sessionRepository = new SessionRepository(_dataConnectionString, 1);
            _transactionRepository = new TransactionRepository(_dataConnectionString, 1);
            _userRepository = new UserRepository(_dataConnectionString, 1);
        }

        [SetUp]
        public void SetUp()
        {
            DeleteTestDatabase(_setupConnectionString);

            // Create the test database
            using (var conn = new SqlConnection(_setupConnectionString))
            {
                conn.Open();
                conn.Execute("CREATE DATABASE [MABMoney_TEST]");
            }

            // Open the test database and create the schema
            using (var conn = new SqlConnection(_dataConnectionString))
            {
                conn.Open();

                var serverConnection = new ServerConnection(conn);
                var server = new Server(serverConnection);

                var schemaSql = File.ReadAllText(_sqlScriptSource + @"\all.sql");

                server.ConnectionContext.ExecuteNonQuery(schemaSql);

                var seedSql = File.ReadAllText(_sqlScriptSource + @"\test.sql");

                server.ConnectionContext.ExecuteNonQuery(seedSql);

                serverConnection.Disconnect();
            }
        }

        #endregion SetUp

        #region Account

        [Test]
        [Category("ServiceLayer_Account")]
        public void Service_Create_Account()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);

            var dto = new AccountDTO {
                Name = "TEST ACCOUNT",
                Type = AccountTypeDTO.Loan,
                DisplayOrder = 203,
                StartingBalance = 25.43M,
                CurrentBalance = 1000M,
                Default = true
            };

            accountServices.Save(dto);

            Assert.IsTrue(dto.AccountID == 6);

            var result = GetSingle<AccountDTO>("SELECT * FROM Accounts WHERE AccountID = 6");

            Assert.IsTrue(result.AccountID == 6);
            Assert.IsTrue(result.Name == "TEST ACCOUNT");
            Assert.IsTrue(result.Type == AccountTypeDTO.Loan);
            Assert.IsTrue(result.DisplayOrder == 203);
            Assert.IsTrue(result.StartingBalance == 25.43M);
            Assert.IsTrue(result.CurrentBalance == 25.43M);
            Assert.IsTrue(result.Default == true);
        }

        [Test]
        [Category("ServiceLayer_Account")]
        public void Service_Read_Accounts()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);

            var data = accountServices.All().ToList();

            Assert.IsTrue(data.Count == 3);
            Assert.IsTrue(data[0].AccountID == 1);
            Assert.IsTrue(data[0].Name == "Current");
            Assert.IsTrue(data[1].AccountID == 2);
            Assert.IsTrue(data[1].Name == "Savings");
            Assert.IsTrue(data[2].AccountID == 3);
            Assert.IsTrue(data[2].Name == "Credit");
        }

        [Test]
        [Category("ServiceLayer_Account")]
        public void Service_Read_Account()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);

            var result = accountServices.Get(2);

            Assert.IsTrue(result.AccountID == 2);
            Assert.IsTrue(result.Name == "Savings");
            Assert.IsTrue(result.Type == AccountTypeDTO.Savings);
            Assert.IsTrue(result.DisplayOrder == 200);
            Assert.IsTrue(result.StartingBalance == 500.00M);
            Assert.IsTrue(result.CurrentBalance == 550.00M);
            Assert.IsTrue(result.Default == false);
        }

        [Test]
        [Category("ServiceLayer_Account")]
        public void Service_Read_Deleted_Account()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);

            var result = accountServices.Get(4);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Account")]
        public void Service_Read_Other_User_Account()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);

            var result = accountServices.Get(5);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Account")]
        public void Service_Update_Account()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);

            var dto = new AccountDTO {
                AccountID = 1,
                Name = "UPDATED",
                Type = AccountTypeDTO.CreditCard,
                DisplayOrder = 156,
                StartingBalance = 200.50M,
                CurrentBalance = 2000M,
                Default = false
            };

            accountServices.Save(dto);

            Assert.IsTrue(dto.AccountID == 1);

            var result = GetSingle<Account>("SELECT * FROM Accounts WHERE AccountID = 1");

            Assert.IsTrue(result.AccountID == 1);
            Assert.IsTrue(result.Name == "UPDATED");
            Assert.IsTrue(result.Type == AccountType.CreditCard);
            Assert.IsTrue(result.DisplayOrder == 156);
            Assert.IsTrue(result.StartingBalance == 200.50M);
            Assert.IsTrue(result.CurrentBalance == 445.50M);
            Assert.IsTrue(result.Default == false);
        }

        [Test]
        [Category("ServiceLayer_Account")]
        public void Service_Delete_Account()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);

            accountServices.Delete(1);

            var account = accountServices.Get(1);

            var result = GetSingle<Account>("SELECT * FROM Accounts WHERE AccountID = 1");

            Assert.IsTrue(account == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        #endregion Account

        #region Budget

        [Test]
        [Category("ServiceLayer_Budget")]
        public void Service_Create_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            var dto = new BudgetDTO {
                Account_AccountID = 1,
                Start = DateTime.Now.Date,
                End = DateTime.Now.AddMonths(1).Date
            };

            budgetServices.Save(dto);

            Assert.IsTrue(dto.BudgetID == 5);

            var result = GetSingle<BudgetDTO>("SELECT * FROM Budgets WHERE BudgetID = 5");

            Assert.IsTrue(result.BudgetID == 5);
            Assert.IsTrue(result.Account_AccountID == 1);
            Assert.IsTrue(result.Start == DateTime.Now.Date);
            Assert.IsTrue(result.End == DateTime.Now.AddMonths(1).Date);
        }

        [Test]
        [Category("ServiceLayer_Budget")]
        public void Service_Read_Budgets()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            var data = budgetServices.All().ToList();

            Assert.IsTrue(data.Count == 2);
            Assert.IsTrue(data[0].BudgetID == 2);
            Assert.IsTrue(data[0].Account_AccountID == 1);
            Assert.IsTrue(data[0].Start == new DateTime(2015, 2, 1));
            Assert.IsTrue(data[0].End == new DateTime(2015, 2, 28));
            Assert.IsTrue(data[1].BudgetID == 1);
            Assert.IsTrue(data[1].Account_AccountID == 1);
            Assert.IsTrue(data[1].Start == new DateTime(2015, 1, 1));
            Assert.IsTrue(data[1].End == new DateTime(2015, 1, 31));
        }

        [Test]
        [Category("ServiceLayer_Budget")]
        public void Service_Read_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            var result = budgetServices.Get(1);

            Assert.IsTrue(result.BudgetID == 1);
            Assert.IsTrue(result.Account_AccountID == 1);
            Assert.IsTrue(result.Start == new DateTime(2015, 01, 01, 0, 0, 0));
            Assert.IsTrue(result.End == new DateTime(2015, 01, 31, 0, 0, 0));
            // There should be two categories (3 in DB but one deleted) and one uncategorised
            Assert.IsTrue(result.Category_Budgets.Count == 3);
            Assert.IsTrue(result.Category_Budgets[0].CategoryName == "Rent");
            Assert.IsTrue(result.Category_Budgets[0].Amount == 500.00M);
            Assert.IsTrue(result.Category_Budgets[0].Total == 500.00M);
            Assert.IsTrue(result.Category_Budgets[1].CategoryName == "Food");
            Assert.IsTrue(result.Category_Budgets[1].Amount == 250.00M);
            Assert.IsTrue(result.Category_Budgets[1].Total == 145.50M);
            Assert.IsTrue(result.Category_Budgets[2].CategoryName == "Unallocated");
            Assert.IsTrue(result.Category_Budgets[2].Amount == 250.00M);
            Assert.IsTrue(result.Category_Budgets[2].Total == 5.00M);
        }

        [Test]
        [Category("ServiceLayer_Budget")]
        public void Service_Read_Latest_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            var result = budgetServices.GetLatest(new DateTime(2015, 02, 01), 1);

            Assert.IsTrue(result.BudgetID == 2);
            Assert.IsTrue(result.Account_AccountID == 1);
            Assert.IsTrue(result.Start == new DateTime(2015, 02, 01, 0, 0, 0));
            Assert.IsTrue(result.End == new DateTime(2015, 02, 28, 0, 0, 0));
            Assert.IsTrue(result.Category_Budgets == null);
        }

        [Test]
        [Category("ServiceLayer_Budget")]
        public void Service_Read_Deleted_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            var result = budgetServices.Get(3);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Budget")]
        public void Service_Read_Other_User_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            var result = budgetServices.Get(4);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Budget")]
        public void Service_Update_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            var dto = new BudgetDTO {
                BudgetID = 1,
                Account_AccountID = 2,
                Start = DateTime.Now.Date,
                End = DateTime.Now.AddMonths(1).Date
            };

            budgetServices.Save(dto);

            Assert.IsTrue(dto.BudgetID == 1);

            var result = GetSingle<BudgetDTO>("SELECT * FROM Budgets WHERE BudgetID = 1");

            Assert.IsTrue(result.BudgetID == 1);
            Assert.IsTrue(result.Account_AccountID == 2);
            Assert.IsTrue(result.Start == DateTime.Now.Date);
            Assert.IsTrue(result.End == DateTime.Now.AddMonths(1).Date);
        }

        [Test]
        [Category("ServiceLayer_Budget")]
        public void Service_Delete_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            budgetServices.Delete(1);

            var budget = budgetServices.Get(1);

            var result = GetSingle<Budget>("SELECT * FROM Budgets WHERE BudgetID = 1");

            Assert.IsTrue(budget == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        #endregion Budget

        #region Budget

        [Test]
        [Category("ServiceLayer_Category_Budget")]
        public void Service_Create_Category_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            throw new NotImplementedException();

            //var dto = new Category_BudgetDTO {
            //    Budget_BudgetID = 1,
            //    Account_AccountID = 1,
            //    Category_CategoryID = 1,
            //    CategoryName = "Salary",
            //    Amount = "1000"
            //};

            //budgetServices.SaveCategoryBudget(dto);

            //Assert.IsTrue(dto.BudgetID == 5);

            //var result = GetSingle<BudgetDTO>("SELECT * FROM Budgets WHERE BudgetID = 5");

            //Assert.IsTrue(result.BudgetID == 5);
            //Assert.IsTrue(result.Account_AccountID == 1);
            //Assert.IsTrue(result.Start == DateTime.Now.Date);
            //Assert.IsTrue(result.End == DateTime.Now.AddMonths(1).Date);
        }

        [Test]
        [Category("ServiceLayer_Category_Budget")]
        public void Service_Read_Category_Budgets()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            throw new NotImplementedException();

            var data = budgetServices.All().ToList();

            Assert.IsTrue(data.Count == 2);
            Assert.IsTrue(data[0].BudgetID == 2);
            Assert.IsTrue(data[0].Account_AccountID == 1);
            Assert.IsTrue(data[0].Start == new DateTime(2015, 2, 1));
            Assert.IsTrue(data[0].End == new DateTime(2015, 2, 28));
            Assert.IsTrue(data[1].BudgetID == 1);
            Assert.IsTrue(data[1].Account_AccountID == 1);
            Assert.IsTrue(data[1].Start == new DateTime(2015, 1, 1));
            Assert.IsTrue(data[1].End == new DateTime(2015, 1, 31));
        }

        [Test]
        [Category("ServiceLayer_Category_Budget")]
        public void Service_Read_Category_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            throw new NotImplementedException();

            var result = budgetServices.Get(1);

            Assert.IsTrue(result.BudgetID == 1);
            Assert.IsTrue(result.Account_AccountID == 1);
            Assert.IsTrue(result.Start == new DateTime(2015, 01, 01, 0, 0, 0));
            Assert.IsTrue(result.End == new DateTime(2015, 01, 31, 0, 0, 0));
            // There should be two categories (3 in DB but one deleted) and one uncategorised
            Assert.IsTrue(result.Category_Budgets.Count == 3);
            Assert.IsTrue(result.Category_Budgets[0].CategoryName == "Rent");
            Assert.IsTrue(result.Category_Budgets[0].Amount == 500.00M);
            Assert.IsTrue(result.Category_Budgets[0].Total == 500.00M);
            Assert.IsTrue(result.Category_Budgets[1].CategoryName == "Food");
            Assert.IsTrue(result.Category_Budgets[1].Amount == 250.00M);
            Assert.IsTrue(result.Category_Budgets[1].Total == 145.50M);
            Assert.IsTrue(result.Category_Budgets[2].CategoryName == "Unallocated");
            Assert.IsTrue(result.Category_Budgets[2].Amount == 250.00M);
            Assert.IsTrue(result.Category_Budgets[2].Total == 5.00M);
        }

        [Test]
        [Category("ServiceLayer_Category_Budget")]
        public void Service_Read_Deleted_Category_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            throw new NotImplementedException();

            var result = budgetServices.Get(3);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Category_Budget")]
        public void Service_Read_Other_User_Category_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            throw new NotImplementedException();

            var result = budgetServices.Get(4);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Category_Budget")]
        public void Service_Update_Category_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            throw new NotImplementedException();

            var dto = new BudgetDTO {
                BudgetID = 1,
                Account_AccountID = 2,
                Start = DateTime.Now.Date,
                End = DateTime.Now.AddMonths(1).Date
            };

            budgetServices.Save(dto);

            Assert.IsTrue(dto.BudgetID == 1);

            var result = GetSingle<BudgetDTO>("SELECT * FROM Budgets WHERE BudgetID = 1");

            Assert.IsTrue(result.BudgetID == 1);
            Assert.IsTrue(result.Account_AccountID == 2);
            Assert.IsTrue(result.Start == DateTime.Now.Date);
            Assert.IsTrue(result.End == DateTime.Now.AddMonths(1).Date);
        }

        [Test]
        [Category("ServiceLayer_Category_Budget")]
        public void Service_Delete_Category_Budget()
        {
            var accountServices = new AccountServices(_accountRepository, _transactionRepository, _userRepository);
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);
            var budgetServices = new BudgetServices(_budgetRepository, _accountRepository, _categoryRepository, _category_budgetRepository, _transactionRepository, accountServices, categoryServices);

            throw new NotImplementedException();

            budgetServices.Delete(1);

            var budget = budgetServices.Get(1);

            var result = GetSingle<Budget>("SELECT * FROM Budgets WHERE BudgetID = 1");

            Assert.IsTrue(budget == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        #endregion Budget

        #region Category

        [Test]
        [Category("ServiceLayer_Category")]
        public void Service_Create_Category()
        {
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);

            var dto = new CategoryDTO {
                Account_AccountID = 1,
                Name = "TEST CATEGORY",
                Type = CategoryTypeDTO.Income,
            };

            categoryServices.Save(dto);

            Assert.IsTrue(dto.CategoryID == 11);

            var result = GetSingle<CategoryDTO>("SELECT * FROM Categories WHERE CategoryID = 11");

            Assert.IsTrue(result.CategoryID == 11);
            Assert.IsTrue(result.Account_AccountID == 1);
            Assert.IsTrue(result.Name == "TEST CATEGORY");
            Assert.IsTrue(result.Type == CategoryTypeDTO.Income);
        }

        [Test]
        [Category("ServiceLayer_Category")]
        public void Service_Read_Categories()
        {
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);

            var data = categoryServices.All().ToList();

            Assert.IsTrue(data.Count == 6);
            Assert.IsTrue(data[0].CategoryID == 1);
            Assert.IsTrue(data[0].Name == "Salary");
            Assert.IsTrue(data[0].Type == CategoryTypeDTO.Income);
            Assert.IsTrue(data[1].CategoryID == 2);
            Assert.IsTrue(data[1].Name == "Rent");
            Assert.IsTrue(data[1].Type == CategoryTypeDTO.Expense);
            Assert.IsTrue(data[2].CategoryID == 3);
            Assert.IsTrue(data[2].Name == "Food");
            Assert.IsTrue(data[2].Type == CategoryTypeDTO.Expense);
            Assert.IsTrue(data[5].CategoryID == 7);
            Assert.IsTrue(data[5].Name == "Bills");
            Assert.IsTrue(data[5].Type == CategoryTypeDTO.Expense);
        }

        [Test]
        [Category("ServiceLayer_Category")]
        public void Service_Read_Category()
        {
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);

            var result = categoryServices.Get(2);

            Assert.IsTrue(result.CategoryID == 2);
            Assert.IsTrue(result.Account_AccountID == 1);
            Assert.IsTrue(result.Name == "Rent");
            Assert.IsTrue(result.Type == CategoryTypeDTO.Expense);
        }

        [Test]
        [Category("ServiceLayer_Category")]
        public void Service_Read_Deleted_Category()
        {
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);

            var result = categoryServices.Get(5);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Category")]
        public void Service_Read_Other_User_Category()
        {
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);

            var result = categoryServices.Get(5);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Category")]
        public void Service_Update_Category()
        {
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);

            var dto = new CategoryDTO {
                CategoryID = 1,
                Account_AccountID = 2,
                Name = "UPDATED",
                Type = CategoryTypeDTO.Expense
            };

            categoryServices.Save(dto);

            Assert.IsTrue(dto.CategoryID == 1);

            var result = GetSingle<Category>("SELECT * FROM Categories WHERE CategoryID = 1");

            Assert.IsTrue(result.CategoryID == 1);
            Assert.IsTrue(result.Account_AccountID == 2);
            Assert.IsTrue(result.Name == "UPDATED");
            Assert.IsTrue(result.Type == CategoryType.Expense);
        }

        [Test]
        [Category("ServiceLayer_Category")]
        public void Service_Delete_Category()
        {
            var categoryServices = new CategoryServices(_categoryRepository, _accountRepository);

            categoryServices.Delete(1);

            var category = categoryServices.Get(1);

            var result = GetSingle<Category>("SELECT * FROM Categories WHERE CategoryID = 1");

            Assert.IsTrue(category == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        #endregion Category

        #region Session

        [Test]
        [Category("ServiceLayer_Session")]
        public void Service_Create_Session()
        {
            var sessionServices = new SessionServices(_sessionRepository);

            var dto = new SessionDTO {
                User_UserID = 1,
                Key = "TESTKEY",
                Expiry = DateTime.Now.AddMonths(1).Date
            };

            sessionServices.Save(dto);

            Assert.IsTrue(dto.SessionID == 5);

            var result = GetSingle<SessionDTO>("SELECT * FROM Sessions WHERE SessionID = 5");

            Assert.IsTrue(result.SessionID == 5);
            Assert.IsTrue(result.User_UserID == 1);
            Assert.IsTrue(result.Key == "TESTKEY");
            Assert.IsTrue(result.Expiry == DateTime.Now.AddMonths(1).Date);
        }

        [Test]
        [Category("ServiceLayer_Session")]
        public void Service_Read_Sessions()
        {
            var sessionServices = new SessionServices(_sessionRepository);

            var data = sessionServices.All().ToList();

            Assert.IsTrue(data.Count == 1);
            Assert.IsTrue(data[0].SessionID == 1);
            Assert.IsTrue(data[0].User_UserID == 1);
            Assert.IsTrue(data[0].Key == "USER1SESSION");
            Assert.IsTrue(data[0].Expiry == new DateTime(2016, 1, 1, 0, 0, 0));
        }

        [Test]
        [Category("ServiceLayer_Session")]
        public void Service_Read_Session()
        {
            var sessionServices = new SessionServices(_sessionRepository);

            var result = sessionServices.Get(1);

            Assert.IsTrue(result.SessionID == 1);
            Assert.IsTrue(result.User_UserID == 1);
            Assert.IsTrue(result.Key == "USER1SESSION");
            Assert.IsTrue(result.Expiry == new DateTime(2016, 1, 1, 0, 0, 0));
        }

        [Test]
        [Category("ServiceLayer_Session")]
        public void Service_Read_Session_By_Key()
        {
            var sessionServices = new SessionServices(_sessionRepository);

            var result = sessionServices.GetByKey("USER1SESSION");

            Assert.IsTrue(result.SessionID == 1);
            Assert.IsTrue(result.User_UserID == 1);
            Assert.IsTrue(result.Key == "USER1SESSION");
            Assert.IsTrue(result.Expiry == new DateTime(2016, 1, 1, 0, 0, 0));
        }

        [Test]
        [Category("ServiceLayer_Session")]
        public void Service_Read_Deleted_Session()
        {
            var sessionServices = new SessionServices(_sessionRepository);

            var result = sessionServices.Get(2);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Session")]
        public void Service_Read_Expired_Session()
        {
            var sessionServices = new SessionServices(_sessionRepository);

            var result = sessionServices.Get(3);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Session")]
        public void Service_Read_Other_User_Session()
        {
            var sessionServices = new SessionServices(_sessionRepository);

            var result = sessionServices.Get(4);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Session")]
        public void Service_Update_Session()
        {
            var sessionServices = new SessionServices(_sessionRepository);

            var dto = new SessionDTO {
                SessionID = 1,
                User_UserID = 2,
                Key = "UPDATED",
                Expiry = DateTime.Now.AddMonths(1).Date
            };

            sessionServices.Save(dto);

            // ID should be new, because you shouldn't be able to update session records
            Assert.IsTrue(dto.SessionID == 5);

            var result = GetSingle<Session>("SELECT * FROM Sessions WHERE SessionID = 5");

            Assert.IsTrue(result.SessionID == 5);
            Assert.IsTrue(result.User_UserID == 2);
            Assert.IsTrue(result.Key == "UPDATED");
            Assert.IsTrue(result.Expiry == DateTime.Now.AddMonths(1).Date);
        }

        [Test]
        [Category("ServiceLayer_Session")]
        public void Service_Delete_Session()
        {
            var sessionServices = new SessionServices(_sessionRepository);

            sessionServices.Delete(1);

            var session = sessionServices.Get(1);

            var result = GetSingle<Session>("SELECT * FROM Sessions WHERE SessionID = 1");

            Assert.IsTrue(session == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        #endregion Session

        #region Transaction

        [Test]
        [Category("ServiceLayer_Transaction")]
        public void Service_Create_Transaction()
        {
            var transactionServices = new TransactionServices(_transactionRepository, _accountRepository);

            var dto = new TransactionDTO {
                Account_AccountID = 1,
                Category_CategoryID = 3,
                Date = new DateTime(2015, 1, 2, 10, 15, 30),
                Description = "Off Licence",
                Note = "Beer",
                Amount = 5.00M
            };

            transactionServices.Save(dto);

            Assert.IsTrue(dto.TransactionID == 32);

            var result = GetSingle<TransactionDTO>("SELECT * FROM Transactions WHERE TransactionID = 32");

            Assert.IsTrue(result.TransactionID == 32);
            Assert.IsTrue(result.Account_AccountID == 1);
            Assert.IsTrue(result.Category_CategoryID == 3);
            Assert.IsTrue(result.Date == new DateTime(2015, 1, 2, 10, 15, 30));
            Assert.IsTrue(result.Description == "Off Licence");
            Assert.IsTrue(result.Note == "Beer");
            Assert.IsTrue(result.Amount == 5.00M);
        }

        [Test]
        [Category("ServiceLayer_Transaction")]
        public void Service_Read_Transactions()
        {
            var transactionServices = new TransactionServices(_transactionRepository, _accountRepository);

            var data = transactionServices.All().ToList();

            Assert.IsTrue(data.Count == 19);
            Assert.IsTrue(data[0].TransactionID == 31);
            Assert.IsTrue(data[0].Description == "USER1CURRENT15");
            Assert.IsTrue(data[0].Amount == -5.00M);
            Assert.IsTrue(data[1].TransactionID == 14);
            Assert.IsTrue(data[1].Description == "USER1CURRENT14");
            Assert.IsTrue(data[1].Amount == -25.00M);
            Assert.IsTrue(data[2].TransactionID == 13);
            Assert.IsTrue(data[2].Description == "USER1CURRENT13");
            Assert.IsTrue(data[2].Amount == -10.00M);
            Assert.IsTrue(data[18].TransactionID == 1);
            Assert.IsTrue(data[18].Description == "USER1CURRENT1");
            Assert.IsTrue(data[18].Note == "Salary");
            Assert.IsTrue(data[18].Amount == 1000.00M);
        }

        [Test]
        [Category("ServiceLayer_Transaction")]
        public void Service_Read_Transaction()
        {
            var transactionServices = new TransactionServices(_transactionRepository, _accountRepository);

            var result = transactionServices.Get(4);

            Assert.IsTrue(result.TransactionID == 4);
            Assert.IsTrue(result.Account_AccountID == 1);
            Assert.IsTrue(result.Category_CategoryID == 4);
            Assert.IsTrue(result.Date == new DateTime(2015, 1, 2, 0, 0, 0));
            Assert.IsTrue(result.Description == "USER1CURRENT4");
            Assert.IsTrue(result.Note == "Gas");
            Assert.IsTrue(result.Amount == -44.50M);
        }

        [Test]
        [Category("ServiceLayer_Transaction")]
        public void Service_Read_Deleted_Transaction()
        {
            var transactionServices = new TransactionServices(_transactionRepository, _accountRepository);

            var result = transactionServices.Get(15);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Transaction")]
        public void Service_Read_Other_User_Transaction()
        {
            var transactionServices = new TransactionServices(_transactionRepository, _accountRepository);

            var result = transactionServices.Get(21);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_Transaction")]
        public void Service_Update_Transaction()
        {
            var transactionServices = new TransactionServices(_transactionRepository, _accountRepository);

            var dto = new TransactionDTO {
                TransactionID = 1,
                Account_AccountID = 2,
                Category_CategoryID = 4,
                Date = new DateTime(2015, 1, 3, 10, 15, 30),
                Description = "UPDATED DESC",
                Note = "UPDATED NOTE",
                Amount = 15.50M
            };

            transactionServices.Save(dto);

            Assert.IsTrue(dto.TransactionID == 1);

            var result = GetSingle<Transaction>("SELECT * FROM Transactions WHERE TransactionID = 1");

            Assert.IsTrue(result.TransactionID == 1);
            Assert.IsTrue(result.Account_AccountID == 2);
            Assert.IsTrue(result.Category_CategoryID == 4);
            Assert.IsTrue(result.Date == new DateTime(2015, 1, 3, 10, 15, 30));
            Assert.IsTrue(result.Description == "UPDATED DESC");
            Assert.IsTrue(result.Note == "UPDATED NOTE");
            Assert.IsTrue(result.Amount == 15.50M);
        }

        [Test]
        [Category("ServiceLayer_Transaction")]
        public void Service_Delete_Transaction()
        {
            var transactionServices = new TransactionServices(_transactionRepository, _accountRepository);

            transactionServices.Delete(1);

            var transaction = transactionServices.Get(1);

            var result = GetSingle<Transaction>("SELECT * FROM Transactions WHERE TransactionID = 1");

            Assert.IsTrue(transaction == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        #endregion Transaction

        #region User

        [Test]
        [Category("ServiceLayer_User")]
        public void Service_Create_User()
        {
            var userServices = new UserServices(_userRepository);

            var dto = new UserDTO {
                Email = "new@test.com",
                Password = "password",
                Forename = "NEW",
                Surname = "USER",
                PasswordResetGUID = Guid.NewGuid().ToString(),
                PasswordResetExpiry = DateTime.Now.AddMonths(1).Date
            };

            userServices.Save(dto);

            Assert.IsTrue(dto.UserID == 3);

            var result = GetSingle<UserDTO>("SELECT * FROM Users WHERE UserID = 3");

            Assert.IsTrue(result.UserID == 3);
            Assert.IsTrue(result.Email == "new@test.com");
            Assert.IsTrue(result.Password == "password");
            Assert.IsTrue(result.Forename == "NEW");
            Assert.IsTrue(result.Surname == "USER");
            Assert.IsTrue(result.PasswordResetGUID == null);
            Assert.IsTrue(result.PasswordResetExpiry == null);
        }

        [Test]
        [Category("ServiceLayer_User")]
        public void Service_Read_User()
        {
            var userServices = new UserServices(_userRepository);

            var result = userServices.Get(1);

            Assert.IsTrue(result.UserID == 1);
            Assert.IsTrue(result.Email == "user@test.com");
            Assert.IsTrue(result.Password == "AJNzdwx56R+U3ls50NZbLTYQBm8j5Txr+F9mz3jQwzNjjIYjIjFuwBr/2l5VnjhQnw==");
            Assert.IsTrue(result.Forename == "Test");
            Assert.IsTrue(result.Surname == "User");
            Assert.IsTrue(result.PasswordResetGUID == "7cc68dbb-3d12-487b-8295-e9b226cda017");
            Assert.IsTrue(result.PasswordResetExpiry == new DateTime(2016, 1, 1));
        }

        [Test]
        [Category("ServiceLayer_User")]
        public void Service_Read_User_By_Email()
        {
            var userServices = new UserServices(_userRepository);

            var result = userServices.GetByEmailAddress("user@test.com");

            Assert.IsTrue(result.UserID == 1);
            Assert.IsTrue(result.Email == "user@test.com");
            Assert.IsTrue(result.Password == "AJNzdwx56R+U3ls50NZbLTYQBm8j5Txr+F9mz3jQwzNjjIYjIjFuwBr/2l5VnjhQnw==");
            Assert.IsTrue(result.Forename == "Test");
            Assert.IsTrue(result.Surname == "User");
            Assert.IsTrue(result.PasswordResetGUID == "7cc68dbb-3d12-487b-8295-e9b226cda017");
            Assert.IsTrue(result.PasswordResetExpiry == new DateTime(2016, 1, 1));
        }

        [Test]
        [Category("ServiceLayer_User")]
        public void Service_Read_Deleted_User()
        {
            var userServices = new UserServices(_userRepository);

            userServices.Delete(1);

            var result = userServices.Get(1);

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_User")]
        public void Service_Read_Deleted_User_By_Email()
        {
            var userServices = new UserServices(_userRepository);

            var result = userServices.GetByEmailAddress("deleted@test.com");

            Assert.IsTrue(result == null);
        }

        [Test]
        [Category("ServiceLayer_User")]
        public void Service_Update_User()
        {
            var userServices = new UserServices(_userRepository);

            var guid = Guid.NewGuid().ToString();

            var dto = new UserDTO {
                UserID = 1,
                Email = "new@test.com",
                Password = "password",
                Forename = "UPDATED",
                Surname = "USER",
                PasswordResetGUID = guid,
                PasswordResetExpiry = DateTime.Now.AddMonths(1).Date
            };

            userServices.Save(dto);

            var result = GetSingle<UserDTO>("SELECT * FROM Users WHERE UserID = 1");

            Assert.IsTrue(result.Email == "new@test.com");
            Assert.IsTrue(result.Password == "password");
            Assert.IsTrue(result.Forename == "UPDATED");
            Assert.IsTrue(result.Surname == "USER");
            Assert.IsTrue(result.PasswordResetGUID == guid);
            Assert.IsTrue(result.PasswordResetExpiry == DateTime.Now.AddMonths(1).Date);
        }

        [Test]
        [Category("ServiceLayer_User")]
        public void Service_Update_Other_User()
        {
            var userServices = new UserServices(_userRepository);

            var dto = new UserDTO {
                UserID = 2,
                Email = "new@test.com",
                Password = "WONTWORK"
            };

            userServices.Save(dto);

            var result = GetSingle<UserDTO>("SELECT * FROM Users WHERE UserID = 1");

            Assert.IsTrue(result.Email == "user@test.com");

            var result2 = GetSingle<UserDTO>("SELECT * FROM Users WHERE UserID = 2");

            Assert.IsTrue(result2.Email == "deleted@test.com");
        }

        [Test]
        [Category("ServiceLayer_User")]
        public void Service_Reset_User_Password()
        {
            var userServices = new UserServices(_userRepository);

            // Plain text password here for ease of comparison
            // Hashing is is usually done in the controller
            userServices.ResetPassword("7cc68dbb-3d12-487b-8295-e9b226cda017", "test123");

            var result = GetSingle<UserDTO>("SELECT * FROM Users WHERE UserID = 1");

            Assert.IsTrue(result.Password == "test123");
            Assert.IsTrue(result.PasswordResetGUID == null);
            Assert.IsTrue(result.PasswordResetExpiry == null);
        }

        [Test]
        [Category("ServiceLayer_User")]
        public void Service_Delete_User()
        {
            var userServices = new UserServices(_userRepository);

            userServices.Delete(1);

            var user = userServices.Get(1);

            var result = GetSingle<MABMoney.Domain.User>("SELECT * FROM Users WHERE UserID = 1");

            Assert.IsTrue(user == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        #endregion User

        #region TearDown

        [TearDown]
        public void TearDown()
        {
            DeleteTestDatabase(_setupConnectionString);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            DeleteTestDatabase(_setupConnectionString);
        }

        #endregion TearDown
    }
}
