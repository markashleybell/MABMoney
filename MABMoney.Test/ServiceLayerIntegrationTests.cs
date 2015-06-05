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

        #region Service Layer Tests

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
            Assert.IsTrue(result.CurrentBalance == 450.50M);
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

        #endregion Service Layer Tests

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
