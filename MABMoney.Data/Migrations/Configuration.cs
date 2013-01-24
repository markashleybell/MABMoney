using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using MABMoney.Data;
using MABMoney.Domain;
using System.Collections.Generic;

namespace MABMoney.Data.Migrations
{
    public class Configuration : DbMigrationsConfiguration<DataStore>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;

            // BE CAREFUL!!
            // AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DataStore context)
        {
            // This method will be called after migrating to the latest version.

            // Only do this if there are no users already in the database
            if(context.Users.Count() == 0)
            {
                var user = new User {
                    Forename = "Mark",
                    Surname = "Bell",
                    Email = "me@markashleybell.com",
                    Password = "ALZByiJAcShmiQpEue7DR0g+RZYSWiSXJoF3VxFck96CEhW8SZakdXaJ+1PDgqGoXw==" // test123
                };

                context.Users.Add(user);

                context.SaveChanges();

                var accounts = new List<Account> {
                    new Account {
                        Name = "Current",
                        User = user,
                        StartingBalance = 0
                    },
                    new Account {
                        Name = "Savings",
                        User = user,
                        StartingBalance = 0
                    },
                    new Account {
                        Name = "Credit Card",
                        User = user,
                        StartingBalance = 0
                    }
                };

                foreach(var account in accounts)
                    context.Accounts.Add(account);

                context.SaveChanges();

                var categories = new List<Category> {
                    new Category {
                        Account = accounts[0],
                        Name = "Salary",
                        Type = CategoryType.Income
                    },
                    new Category {
                        Account = accounts[0],
                        Name = "Rent",
                        Type = CategoryType.Expense
                    },
                    new Category {
                        Account = accounts[0],
                        Name = "Food",
                        Type = CategoryType.Expense
                    },
                    new Category {
                        Account = accounts[0],
                        Name = "Fuel",
                        Type = CategoryType.Expense
                    }
                };

                foreach (var category in categories)
                    context.Categories.Add(category);

                context.SaveChanges();
            }
        }
    }
}
