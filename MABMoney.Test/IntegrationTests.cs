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

namespace MABMoney.Test
{
    [TestFixture]
    public class IntegrationTests
    {
        private string _setupConnectionString;
        private string _dataConnectionString;
        private string _sqlScriptSource;

        #region SetUp

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _setupConnectionString = ConfigurationManager.AppSettings["SetUpDbConnectionString"];
            _dataConnectionString = ConfigurationManager.AppSettings["DataDbConnectionString"];
            _sqlScriptSource = ConfigurationManager.AppSettings["SqlScriptSource"];

            // TODO: Make sure SQL build scripts have been run
        }

        [SetUp]
        public void SetUp()
        {
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

        [Test]
        public void Data_Create_User()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var user = new MABMoney.Domain.User { 
                Forename = "ADDEDFORENAME",
                Surname = "ADDEDSURNAME",
                Email = "added@test.com",
                Password = "AEVg+8Chm8T0NSff0k0qegArPYXetlQfvKEoaDXwnT0N9fj0TVAjorveDX9vfbcVwA==", // "password"
                PasswordResetGUID = "0c4ffa03-e3d7-48b6-b657-bdae23f5d14d",
                PasswordResetExpiry = new DateTime(2015, 1, 1),
                IsAdmin = false
            };

            repository.Add(user);

            Assert.IsTrue(user.UserID == 3);
            Assert.IsTrue(user.CreatedDate.Date == DateTime.Now.Date);
        }

        [Test]
        public void Data_Read_User()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var data = repository.Get();

            Assert.IsTrue(data.UserID == 1);
            Assert.IsTrue(data.Email == "user@test.com");
        }

        [Test]
        public void Data_Read_User_By_Email()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var data = repository.GetByEmailAddress("user@test.com");

            Assert.IsTrue(data.UserID == 1);
            Assert.IsTrue(data.Forename == "Test");
            Assert.IsTrue(data.Surname == "User");
        }

        [Test]
        public void Data_Read_Deleted_User_By_Email()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var data = repository.GetByEmailAddress("deleted@test.com");

            Assert.IsTrue(data == null);
        }

        [Test]
        public void Data_Read_User_By_Password_Reset_GUID()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var data = repository.GetByPasswordResetGUID("7cc68dbb-3d12-487b-8295-e9b226cda017");

            Assert.IsTrue(data.UserID == 1);
            Assert.IsTrue(data.Email == "user@test.com");
        }

        [Test]
        public void Data_Read_Deleted_User_By_Password_Reset_GUID()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var data = repository.GetByPasswordResetGUID("5b977b67-e7e6-4399-9866-6c011750249f");

            Assert.IsTrue(data == null);
        }

        [Test]
        public void Data_Update_User()
        {
            var repository = new UserRepository(_dataConnectionString, 1);

            var user = new MABMoney.Domain.User { 
                Forename = "UPDATEDFORENAME",
                Surname = "UPDATEDSURNAME",
                Email = "added@test.com",
                Password = "AEVg+8Chm8T0NSff0k0qegArPYXetlQfvKEoaDXwnT0N9fj0TVAjorveDX9vfbcVwA==", // "password"
                PasswordResetGUID = "0c4ffa03-e3d7-48b6-b657-bdae23f5d14d",
                PasswordResetExpiry = new DateTime(2015, 1, 1),
                IsAdmin = false
            };

            repository.Update(user);

            Assert.IsTrue(user.UserID == 1);
            Assert.IsTrue(user.LastModifiedDate.Date == DateTime.Now.Date);
        }


        [Test]
        public void Data_Read_All_Accounts()
        {
            var repository = new AccountRepository(_dataConnectionString, 1);

            var data = repository.All();

            // There are actually four accounts in the test SQL, but one is flagged as deleted and so shouldn't be returned
            Assert.IsTrue(data.Count() == 3);
        }

        #endregion Data Layer Tests

        #region TearDown

        [TearDown]
        public void TearDown()
        {
            // Delete the test database
            using (var conn = new SqlConnection(_setupConnectionString))
            {
                conn.Open();

                var sql = @"DECLARE @kill varchar(8000) = '';
                            SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), spid) + ';' FROM master..sysprocesses WHERE dbid = db_id('MABMoney_TEST');
                            EXEC(@kill);
                            DROP DATABASE [MABMoney_TEST];";

                conn.Execute(sql);
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {

        }

        #endregion TearDown
    }
}
