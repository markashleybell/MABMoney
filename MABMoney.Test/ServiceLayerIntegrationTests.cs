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
        [Category("Account")]
        public void Service_Create_Account()
        {
            throw new NotImplementedException();
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
