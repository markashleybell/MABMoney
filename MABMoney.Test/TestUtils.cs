using SmoLiteApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Test
{
    class TestUtils
    {
        public static void DeleteTestDatabase(string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var server = new Server(conn);

                if (server.Databases["MABMoney_TEST"] != null)
                {
                    server.KillAllProcesses("MABMoney_TEST");
                    server.KillDatabase("MABMoney_TEST");
                }
            }
        }

        public static void CreateTestDatabase(string connectionString, string sqlScriptSource)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var server = new Server(conn);

                var database = new Database(server, "MABMoney_TEST");
                database.Create();

                var schemaSql = File.ReadAllText(sqlScriptSource + @"\all.sql");

                database.ExecuteNonQuery(schemaSql);

                var seedSql = File.ReadAllText(sqlScriptSource + @"\test.sql");

                database.ExecuteNonQuery(seedSql);
            }
        }
    }
}
