using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using MABMoney.Data.Concrete;

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
