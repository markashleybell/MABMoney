using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Dapper;
using MABMoney.Data.Concrete;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using NUnit.Framework;
using MABMoney.Domain;

namespace MABMoney.Test
{
    [TestFixture]
    public class IntegrationTests
    {
        private string _setupConnectionString;
        private string _dataConnectionString;
        private string _sqlScriptSource;

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
        }

        [SetUp]
        public void SetUp()
        {
            DeleteTestDatabase(_setupConnectionString);

            // Create the test database
            using(var conn = new SqlConnection(_setupConnectionString))
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

        #region Data Layer Tests

        #region Account

        [Test]
        [Category("Account")]
        public void Data_Create_Account()
        {
            var repository = new AccountRepository(_dataConnectionString, 1);

            var account = new MABMoney.Domain.Account {
                Name = "ADDED",
                StartingBalance = 123.45M,
                Default = true,
                Type = AccountType.Savings,
                DisplayOrder = 50
            };

            var result = repository.Add(account);

            Assert.IsTrue(result.AccountID == 6);
            Assert.IsTrue(result.Name == "ADDED");
            Assert.IsTrue(result.StartingBalance == 123.45M);
            Assert.IsTrue(result.Default == true);
            Assert.IsTrue(result.Type == AccountType.Savings);
            Assert.IsTrue(result.DisplayOrder == 50);
            Assert.IsTrue(result.User_UserID == 1);
            Assert.IsTrue(result.CreatedBy == 1);
            Assert.IsTrue(result.CreatedDate.Date == DateTime.Now.Date);
            Assert.IsTrue(result.LastModifiedBy == 1);
            Assert.IsTrue(result.LastModifiedDate.Date == DateTime.Now.Date);
            Assert.IsTrue(result.Type == AccountType.Savings);
            Assert.IsTrue(result.CurrentBalance == 123.45M);
        }

        [Test]
        [Category("Account")]
        public void Data_Read_Accounts()
        {
            var repository = new AccountRepository(_dataConnectionString, 1);

            var data = repository.All().ToList();

            // There are 5 test accounts, but one is deleted and one is for another user
            Assert.IsTrue(data.Count == 3);
            Assert.IsTrue(data[0].AccountID == 1);
            Assert.IsTrue(data[0].Name == "Current");
            Assert.IsTrue(data[1].AccountID == 2);
            Assert.IsTrue(data[1].Name == "Savings");
            Assert.IsTrue(data[2].AccountID == 3);
            Assert.IsTrue(data[2].Name == "Credit");
        }

        [Test]
        [Category("Account")]
        public void Data_Read_Account()
        {
            var repository = new AccountRepository(_dataConnectionString, 1);

            var data = repository.Get(1);

            Assert.IsTrue(data.AccountID == 1);
            Assert.IsTrue(data.Name == "Current");
        }

        [Test]
        [Category("Account")]
        public void Data_Read_Deleted_Account()
        {
            var repository = new AccountRepository(_dataConnectionString, 1);

            var data = repository.Get(4);

            Assert.IsTrue(data == null);
        }

        [Test]
        [Category("Account")]
        public void Data_Update_Account()
        {
            var repository = new AccountRepository(_dataConnectionString, 1);

            var account = repository.Get(1);

            account.Name = "UPDATED";
            account.StartingBalance = 256.12M;
            account.Default = false;
            account.Type = AccountType.CreditCard;
            account.DisplayOrder = 126;

            var result = repository.Update(account);

            Assert.IsTrue(result.Name == "UPDATED");
            Assert.IsTrue(result.StartingBalance == 256.12M);
            Assert.IsTrue(result.Default == false);
            Assert.IsTrue(result.Type == AccountType.CreditCard);
            Assert.IsTrue(result.DisplayOrder == 126);
            // This will have been updated by the total calc trigger when the test transactions are inserted
            Assert.IsTrue(result.CurrentBalance == 506.12M); 
            Assert.IsTrue(result.LastModifiedBy == 1);
            Assert.IsTrue(result.LastModifiedDate.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("Account")]
        public void Data_Delete_Account()
        {
            var repository = new AccountRepository(_dataConnectionString, 1);

            var result = repository.Delete(1);

            var account = repository.Get(1);

            Assert.IsTrue(account == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        #endregion Account

        #region Budget

        [Test]
        [Category("Budget")]
        public void Data_Create_Budget()
        {
            var repository = new BudgetRepository(_dataConnectionString, 1);

            var budget = new MABMoney.Domain.Budget {
                Account_AccountID = 2,
                Start = new DateTime(2015, 3, 1),
                End = new DateTime(2015, 3, 31)
            };

            var result = repository.Add(budget);

            Assert.IsTrue(result.BudgetID == 5);
            Assert.IsTrue(result.Account_AccountID == 2);
            Assert.IsTrue(result.Start == new DateTime(2015, 3, 1));
            Assert.IsTrue(result.End == new DateTime(2015, 3, 31));
            Assert.IsTrue(result.CreatedBy == 1);
            Assert.IsTrue(result.CreatedDate.Date == DateTime.Now.Date);
            Assert.IsTrue(result.LastModifiedBy == 1);
            Assert.IsTrue(result.LastModifiedDate.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("Budget")]
        public void Data_Read_Budgets()
        {
            var repository = new BudgetRepository(_dataConnectionString, 1);

            var data = repository.All().ToList();

            // There are 4 test budgets, but one is deleted and one is for another user
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
        [Category("Budget")]
        public void Data_Read_Budget()
        {
            var repository = new BudgetRepository(_dataConnectionString, 1);

            var data = repository.Get(2);

            Assert.IsTrue(data.BudgetID == 2);
            Assert.IsTrue(data.Account_AccountID == 1);
            Assert.IsTrue(data.Start == new DateTime(2015, 2, 1));
            Assert.IsTrue(data.End == new DateTime(2015, 2, 28));
        }

        [Test]
        [Category("Budget")]
        public void Data_Read_Deleted_Budget()
        {
            var repository = new BudgetRepository(_dataConnectionString, 1);

            var data = repository.Get(3);

            Assert.IsTrue(data == null);
        }

        [Test]
        [Category("Budget")]
        public void Data_Update_Budget()
        {
            var repository = new BudgetRepository(_dataConnectionString, 1);

            var budget = repository.Get(1);

            budget.Account_AccountID = 2;
            budget.Start = new DateTime(2015, 4, 1);
            budget.End = new DateTime(2015, 4, 30);

            var result = repository.Update(budget);

            Assert.IsTrue(result.BudgetID == 1);
            Assert.IsTrue(result.Account_AccountID == 2);
            Assert.IsTrue(result.Start == new DateTime(2015, 4, 1));
            Assert.IsTrue(result.End == new DateTime(2015, 4, 30));
            Assert.IsTrue(result.LastModifiedBy == 1);
            Assert.IsTrue(result.LastModifiedDate.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("Budget")]
        public void Data_Delete_Budget()
        {
            var repository = new BudgetRepository(_dataConnectionString, 1);

            var result = repository.Delete(1);

            var budget = repository.Get(1);

            Assert.IsTrue(budget == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        #endregion Budget

        #region Category

        [Test]
        [Category("Category")]
        public void Data_Create_Category()
        {
            var repository = new CategoryRepository(_dataConnectionString, 1);

            var category = new MABMoney.Domain.Category {
                Account_AccountID = 1,
                Name = "ADDED",
                Type = CategoryType.Income
            };

            var result = repository.Add(category);

            Assert.IsTrue(result.CategoryID == 11);
            Assert.IsTrue(result.Name == "ADDED");
            Assert.IsTrue(result.Type == CategoryType.Income);
            Assert.IsTrue(result.Account_AccountID == 1);
            Assert.IsTrue(result.CreatedBy == 1);
            Assert.IsTrue(result.CreatedDate.Date == DateTime.Now.Date);
            Assert.IsTrue(result.LastModifiedBy == 1);
            Assert.IsTrue(result.LastModifiedDate.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("Category")]
        public void Data_Read_Categories()
        {
            var repository = new CategoryRepository(_dataConnectionString, 1);

            var data = repository.All(1).OrderBy(x => x.CategoryID).ToList();

            // There are 10 test categories, but ony 4 for this account and user
            Assert.IsTrue(data.Count == 4);
            Assert.IsTrue(data[0].CategoryID == 1);
            Assert.IsTrue(data[0].Name == "Salary");
            Assert.IsTrue(data[1].CategoryID == 2);
            Assert.IsTrue(data[1].Name == "Rent");
            Assert.IsTrue(data[2].CategoryID == 3);
            Assert.IsTrue(data[2].Name == "Food");
            Assert.IsTrue(data[3].CategoryID == 4);
            Assert.IsTrue(data[3].Name == "Bills");
        }

        [Test]
        [Category("Category")]
        public void Data_Read_Category()
        {
            var repository = new CategoryRepository(_dataConnectionString, 1);

            var data = repository.Get(1);

            Assert.IsTrue(data.CategoryID == 1);
            Assert.IsTrue(data.Name == "Salary");
        }

        [Test]
        [Category("Category")]
        public void Data_Read_Deleted_Category()
        {
            var repository = new CategoryRepository(_dataConnectionString, 1);

            var data = repository.Get(5);

            Assert.IsTrue(data == null);
        }

        [Test]
        [Category("Category")]
        public void Data_Update_Category()
        {
            var repository = new CategoryRepository(_dataConnectionString, 1);

            var category = repository.Get(1);

            category.Account_AccountID = 2;
            category.Name = "UPDATED";
            category.Type = CategoryType.Income;

            var result = repository.Update(category);

            Assert.IsTrue(result.Account_AccountID == 2);
            Assert.IsTrue(result.Name == "UPDATED");
            Assert.IsTrue(result.Type == CategoryType.Income);
            Assert.IsTrue(result.LastModifiedBy == 1);
            Assert.IsTrue(result.LastModifiedDate.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("Category")]
        public void Data_Delete_Category()
        {
            var repository = new CategoryRepository(_dataConnectionString, 1);

            var result = repository.Delete(1);

            var category = repository.Get(1);

            Assert.IsTrue(category == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        #endregion Category

        #region Session

        [Test]
        [Category("Session")]
        public void Data_Create_Session()
        {
            var repository = new SessionRepository(_dataConnectionString, 1);

            var session = new MABMoney.Domain.Session {
                Key = "ADDED",
                Expiry = new DateTime(2016, 1, 1)
            };

            var result = repository.Add(session);

            Assert.IsTrue(result.SessionID == 5);
            Assert.IsTrue(result.User_UserID == 1);
            Assert.IsTrue(result.Key == "ADDED");
            Assert.IsTrue(result.Expiry.Date == new DateTime(2016, 1, 1).Date);
            Assert.IsTrue(result.CreatedBy == 1);
            Assert.IsTrue(result.CreatedDate.Date == DateTime.Now.Date);
            Assert.IsTrue(result.LastModifiedBy == 1);
            Assert.IsTrue(result.LastModifiedDate.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("Session")]
        public void Data_Read_Sessions()
        {
            var repository = new SessionRepository(_dataConnectionString, 1);

            var data = repository.All().OrderBy(x => x.SessionID).ToList();

            // There are 4 test sessions, but only 1 current for this user
            Assert.IsTrue(data.Count == 1);
            Assert.IsTrue(data[0].SessionID == 1);
            Assert.IsTrue(data[0].Key == "USER1SESSION");
        }

        [Test]
        [Category("Session")]
        public void Data_Read_Session()
        {
            var repository = new SessionRepository(_dataConnectionString, 1);

            var data = repository.Get(1);

            Assert.IsTrue(data.SessionID == 1);
            Assert.IsTrue(data.Key == "USER1SESSION");
        }

        [Test]
        [Category("Session")]
        public void Data_Read_Session_By_Key()
        {
            var repository = new SessionRepository(_dataConnectionString, 1);

            var data = repository.GetByKey("USER1SESSION");

            Assert.IsTrue(data.SessionID == 1);
        }

        [Test]
        [Category("Session")]
        public void Data_Read_Deleted_Session()
        {
            var repository = new SessionRepository(_dataConnectionString, 1);

            var data = repository.Get(2);

            Assert.IsTrue(data == null);
        }

        [Test]
        [Category("Session")]
        public void Data_Update_Session()
        {
            var repository = new SessionRepository(_dataConnectionString, 1);

            var session = repository.Get(1);

            session.Key = "UPDATED";
            session.Expiry = new DateTime(2016, 2, 2);

            var result = repository.Update(session);

            Assert.IsTrue(result.Key == "UPDATED");
            Assert.IsTrue(result.Expiry.Date == new DateTime(2016, 2, 2).Date);
            Assert.IsTrue(result.LastModifiedBy == 1);
            Assert.IsTrue(result.LastModifiedDate.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("Session")]
        public void Data_Update_Session_Expiry()
        {
            var repository = new SessionRepository(_dataConnectionString, 1);

            var result = repository.UpdateSessionExpiry("USER1SESSION", new DateTime(2016, 2, 2));

            Assert.IsTrue(result.Key == "USER1SESSION");
            Assert.IsTrue(result.Expiry.Date == new DateTime(2016, 2, 2).Date);
            Assert.IsTrue(result.LastModifiedBy == 1);
            Assert.IsTrue(result.LastModifiedDate.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("Session")]
        public void Data_Delete_Session()
        {
            var repository = new SessionRepository(_dataConnectionString, 1);

            var result = repository.Delete(1);

            var session = repository.Get(1);

            Assert.IsTrue(session == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("Session")]
        public void Data_Delete_Session_By_Key()
        {
            var repository = new SessionRepository(_dataConnectionString, 1);

            var result = repository.DeleteByKey("USER1SESSION");

            var session = repository.Get(1);

            Assert.IsTrue(session == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("Session")]
        public void Data_Delete_Session_Expired()
        {
            var repository = new SessionRepository(_dataConnectionString, 1);

            repository.DeleteExpired();

            var session = repository.Get(2);

            Assert.IsTrue(session == null);

            // Read the database directly and check the expired session is now marked as deleted
            using (var conn = new SqlConnection(_dataConnectionString))
            {
                conn.Open();

                var expiredSession = conn.Query<Session>("SELECT * FROM [dbo].[Sessions] WHERE [Key] = 'USER1SESSIONEXPIRED'").Single();

                Assert.IsTrue(expiredSession.Deleted == true);
                Assert.IsTrue(expiredSession.DeletedBy == 1);
                Assert.IsTrue(expiredSession.DeletedDate.Value.Date == DateTime.Now.Date);
            }
        }

        #endregion Session

        #region Transaction

        [Test]
        [Category("Transaction")]
        public void Data_Create_Transaction()
        {
            var repository = new TransactionRepository(_dataConnectionString, 1);

            var transaction = new MABMoney.Domain.Transaction {
                Account_AccountID = 1,
                Category_CategoryID = 3,
                Description = "ADDED",
                Amount = -10.25M,
                Date = new DateTime(2015, 1, 10, 18, 35, 10, 0),
                Note = "BISCUITS",
                TransferGUID = "5485364b-cac5-4c14-9638-5e7c0235a7c1"
            };

            var result = repository.Add(transaction);

            Assert.IsTrue(result.TransactionID == 31);
            Assert.IsTrue(result.Account_AccountID == 1);
            Assert.IsTrue(result.Category_CategoryID == 3);
            Assert.IsTrue(result.Description == "ADDED");
            Assert.IsTrue(result.Amount == -10.25M);
            Assert.IsTrue(result.Date == new DateTime(2015, 1, 10, 18, 35, 10, 0));
            Assert.IsTrue(result.Note == "BISCUITS");
            Assert.IsTrue(result.TransferGUID == "5485364b-cac5-4c14-9638-5e7c0235a7c1");
            Assert.IsTrue(result.CreatedBy == 1);
            Assert.IsTrue(result.CreatedDate.Date == DateTime.Now.Date);
            Assert.IsTrue(result.LastModifiedBy == 1);
            Assert.IsTrue(result.LastModifiedDate.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("Transaction")]
        public void Data_Read_Transactions()
        {
            var repository = new TransactionRepository(_dataConnectionString, 1);

            var data = repository.All().ToList();

            // There are 18 test transactions for this user (and one deleted) in date descending order
            Assert.IsTrue(data.Count == 18);
            Assert.IsTrue(data[0].Account_AccountID == 1);
            Assert.IsTrue(data[0].TransactionID == 14);
            Assert.IsTrue(data[0].Description == "USER1CURRENT14");
            Assert.IsTrue(data[0].Note == "Water");
            Assert.IsTrue(data[5].Account_AccountID == 2);
            Assert.IsTrue(data[5].TransactionID == 19);
            Assert.IsTrue(data[5].Description == "USER1SAVINGS19");
            Assert.IsTrue(data[17].Account_AccountID == 1);
            Assert.IsTrue(data[17].TransactionID == 1);
            Assert.IsTrue(data[17].Description == "USER1CURRENT1");
        }

        [Test]
        [Category("Transaction")]
        public void Data_Read_Transactions_By_Account()
        {
            var repository = new TransactionRepository(_dataConnectionString, 1);

            var data = repository.GetForAccount(1).ToList();

            // There are 14 test transactions for this user (and one deleted) in date descending order
            Assert.IsTrue(data.Count == 14);
            Assert.IsTrue(data[0].TransactionID == 14);
            Assert.IsTrue(data[0].Description == "USER1CURRENT14");
            Assert.IsTrue(data[0].Note == "Water");
            Assert.IsTrue(data[13].TransactionID == 1);
            Assert.IsTrue(data[13].Description == "USER1CURRENT1");
        }

        [Test]
        [Category("Transaction")]
        public void Data_Read_Transactions_By_Account_And_Date()
        {
            var repository = new TransactionRepository(_dataConnectionString, 1);

            var data = repository.GetForAccount(1, new DateTime(2015, 01, 02), new DateTime(2015, 01, 04)).ToList();

            Assert.IsTrue(data.Count == 7);
            Assert.IsTrue(data[0].TransactionID == 10);
            Assert.IsTrue(data[0].Description == "USER1CURRENT10");
            Assert.IsTrue(data[6].TransactionID == 4);
            Assert.IsTrue(data[6].Description == "USER1CURRENT4");
            Assert.IsTrue(data[6].Note == "Gas");
        }

        [Test]
        [Category("Transaction")]
        public void Data_Read_Transactions_By_Account_Date_And_Category()
        {
            var repository = new TransactionRepository(_dataConnectionString, 1);

            var data = repository.GetForAccount(1, 4, new DateTime(2015, 01, 02), new DateTime(2015, 01, 06)).ToList();

            Assert.IsTrue(data.Count == 4);
            Assert.IsTrue(data[0].TransactionID == 14);
            Assert.IsTrue(data[0].Description == "USER1CURRENT14");
            Assert.IsTrue(data[0].Note == "Water");
            Assert.IsTrue(data[1].TransactionID == 11);
            Assert.IsTrue(data[1].Description == "USER1CURRENT11");
            Assert.IsTrue(data[1].Note == "Electricity");
            Assert.IsTrue(data[2].TransactionID == 7);
            Assert.IsTrue(data[2].Description == "USER1CURRENT7");
            Assert.IsTrue(data[2].Note == "Mobile");
            Assert.IsTrue(data[3].TransactionID == 4);
            Assert.IsTrue(data[3].Description == "USER1CURRENT4");
            Assert.IsTrue(data[3].Note == "Gas");
        }

        [Test]
        [Category("Transaction")]
        public void Data_Read_Transaction()
        {
            var repository = new TransactionRepository(_dataConnectionString, 1);

            var data = repository.Get(1);

            Assert.IsTrue(data.TransactionID == 1);
            Assert.IsTrue(data.Description == "USER1CURRENT1");
        }

        [Test]
        [Category("Transaction")]
        public void Data_Read_Deleted_Transaction()
        {
            var repository = new TransactionRepository(_dataConnectionString, 1);

            var data = repository.Get(15);

            Assert.IsTrue(data == null);
        }

        [Test]
        [Category("Transaction")]
        public void Data_Update_Transaction()
        {
            var repository = new TransactionRepository(_dataConnectionString, 1);

            var transaction = repository.Get(1);

            transaction.Account_AccountID = 3;
            transaction.Category_CategoryID = 6;
            transaction.Description = "UPDATED";
            transaction.Amount = -30.00M;
            transaction.Date = new DateTime(2015, 1, 11, 11, 40, 30, 0);
            transaction.Note = "MOVEDTOCREDIT";
            transaction.TransferGUID = "5485364b-cac5-4c14-9638-5e7c0235a7c1";

            var result = repository.Update(transaction);

            Assert.IsTrue(result.TransactionID == 1);
            Assert.IsTrue(result.Account_AccountID == 3);
            Assert.IsTrue(result.Category_CategoryID == 6);
            Assert.IsTrue(result.Description == "UPDATED");
            Assert.IsTrue(result.Amount == -30.00M);
            Assert.IsTrue(result.Date == new DateTime(2015, 1, 11, 11, 40, 30, 0));
            Assert.IsTrue(result.Note == "MOVEDTOCREDIT");
            Assert.IsTrue(result.TransferGUID == "5485364b-cac5-4c14-9638-5e7c0235a7c1");
            Assert.IsTrue(result.LastModifiedBy == 1);
            Assert.IsTrue(result.LastModifiedDate.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("Transaction")]
        public void Data_Delete_Transaction()
        {
            var repository = new TransactionRepository(_dataConnectionString, 1);

            var result = repository.Delete(1);

            var transaction = repository.Get(1);

            Assert.IsTrue(transaction == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        #endregion Transaction

        #region User

        [Test]
        [Category("User")]
        public void Data_Create_User()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var user = new MABMoney.Domain.User
            {
                Forename = "ADDEDFORENAME",
                Surname = "ADDEDSURNAME",
                Email = "added@test.com",
                Password = "AEVg+8Chm8T0NSff0k0qegArPYXetlQfvKEoaDXwnT0N9fj0TVAjorveDX9vfbcVwA==", // "password"
                IsAdmin = false
            };

            var result = repository.Add(user);

            Assert.IsTrue(result.UserID == 3);
            Assert.IsTrue(result.Forename == "ADDEDFORENAME");
            Assert.IsTrue(result.Surname == "ADDEDSURNAME");
            Assert.IsTrue(result.Email == "added@test.com");
            Assert.IsTrue(result.Password == "AEVg+8Chm8T0NSff0k0qegArPYXetlQfvKEoaDXwnT0N9fj0TVAjorveDX9vfbcVwA==");
            Assert.IsTrue(result.IsAdmin == false);
            Assert.IsTrue(result.CreatedDate.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("User")]
        public void Data_Read_User()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var data = repository.Get();

            Assert.IsTrue(data.UserID == 1);
            Assert.IsTrue(data.Email == "user@test.com");
        }

        [Test]
        [Category("User")]
        public void Data_Read_User_By_Email()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var data = repository.GetByEmailAddress("user@test.com");

            Assert.IsTrue(data.UserID == 1);
            Assert.IsTrue(data.Forename == "Test");
            Assert.IsTrue(data.Surname == "User");
        }

        [Test]
        [Category("User")]
        public void Data_Read_Deleted_User_By_Email()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var data = repository.GetByEmailAddress("deleted@test.com");

            Assert.IsTrue(data == null);
        }

        [Test]
        [Category("User")]
        public void Data_Read_User_By_Password_Reset_GUID()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var data = repository.GetByPasswordResetGUID("7cc68dbb-3d12-487b-8295-e9b226cda017");

            Assert.IsTrue(data.UserID == 1);
            Assert.IsTrue(data.Email == "user@test.com");
        }

        [Test]
        [Category("User")]
        public void Data_Read_Deleted_User_By_Password_Reset_GUID()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var data = repository.GetByPasswordResetGUID("5b977b67-e7e6-4399-9866-6c011750249f");

            Assert.IsTrue(data == null);
        }

        [Test]
        [Category("User")]
        public void Data_Update_User()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var user = repository.Get();

            user.Forename = "UPDATEDFORENAME";
            user.Surname = "UPDATEDSURNAME";
            user.Email = "added@test.com";
            user.Password = "AEVg+8Chm8T0NSff0k0qegArPYXetlQfvKEoaDXwnT0N9fj0TVAjorveDX9vfbcVwA=="; // "password"
            user.PasswordResetGUID = "0c4ffa03-e3d7-48b6-b657-bdae23f5d14d";
            user.PasswordResetExpiry = new DateTime(2015, 1, 1);
            user.IsAdmin = false;

            var result = repository.Update(user);

            Assert.IsTrue(result.Forename == "UPDATEDFORENAME");
            Assert.IsTrue(result.Surname == "UPDATEDSURNAME");
            Assert.IsTrue(result.Email == "added@test.com");
            Assert.IsTrue(result.Password == "AEVg+8Chm8T0NSff0k0qegArPYXetlQfvKEoaDXwnT0N9fj0TVAjorveDX9vfbcVwA==");
            Assert.IsTrue(result.PasswordResetGUID == "0c4ffa03-e3d7-48b6-b657-bdae23f5d14d");
            Assert.IsTrue(result.PasswordResetExpiry == new DateTime(2015, 1, 1));
            Assert.IsTrue(result.IsAdmin == false);
            Assert.IsTrue(result.LastModifiedBy == 1);
            Assert.IsTrue(result.LastModifiedDate.Date == DateTime.Now.Date);
        }

        [Test]
        [Category("User")]
        public void Data_Delete_User()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var result = repository.Delete();

            var user = repository.Get();

            Assert.IsTrue(user == null);
            Assert.IsTrue(result.Deleted == true);
            Assert.IsTrue(result.DeletedBy == 1);
            Assert.IsTrue(result.DeletedDate.Value.Date == DateTime.Now.Date);
        }

        #endregion User

        #endregion Data Layer Tests

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
