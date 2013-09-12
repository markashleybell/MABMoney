using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MABMoney.Data.Migrations;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;

namespace MABMoney.Data
{
    public class MigrateDatabaseToLatestVersionWithConnectionString<TContext, TMigrationsConfiguration> : IDatabaseInitializer<TContext>
        where TContext : DbContext
        where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()
    {
        private readonly DbMigrationsConfiguration _config;

        public MigrateDatabaseToLatestVersionWithConnectionString()
        {
            _config = new TMigrationsConfiguration();
        }

        public MigrateDatabaseToLatestVersionWithConnectionString(string connectionString)
        {
            _config = new TMigrationsConfiguration { 
                TargetDatabase = new DbConnectionInfo(connectionString, "System.Data.SqlClient")
            };
        }

        public void InitializeDatabase(TContext context)
        {
            DbMigrator dbMigrator = new DbMigrator(_config);
            dbMigrator.Update();
        }
    }
}
