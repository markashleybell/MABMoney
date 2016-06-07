using MABMoney.Domain.Abstract;
using MABMoney.Domain.Extensions;
using System;

namespace MABMoney.Domain
{
    public class Factory : IFactory
    {
        public Account CreateAccount(int _userId, string name, AccountType type, decimal startingBalance)
        {
            return CreateAccount(_userId, name, type, startingBalance, false, -1, true);
        }

        public Account CreateAccount(int _userId, string name, AccountType type, decimal startingBalance, bool isDefaultAccount)
        {
            return CreateAccount(_userId, name, type, startingBalance, isDefaultAccount, -1, true);
        }

        public Account CreateAccount(int _userId, string name, AccountType type, decimal startingBalance, bool isDefaultAccount, int displayOrder)
        {
            return CreateAccount(_userId, name, type, startingBalance, isDefaultAccount, displayOrder, true);
        }

        public Account CreateAccount(int _userId, string name, AccountType type, decimal startingBalance, bool isDefaultAccount, int displayOrder, bool includeInNetWorth)
        {
            var account = new Account();

            // Set public properties
            account.Name = name;
            account.Default = isDefaultAccount;
            account.DisplayOrder = displayOrder;
            account.IncludeInNetWorth = includeInNetWorth;

            // Set private properties
            account.SetPrivatePropertyValue("User_UserID", _userId);
            account.SetPrivatePropertyValue("Type", type);
            account.SetPrivatePropertyValue("StartingBalance", startingBalance);
            account.SetPrivatePropertyValue("CurrentBalance", startingBalance);

            // Set private audit properties
            var now = DateTime.Now;

            account.SetPrivatePropertyValue("CreatedBy", _userId);
            account.SetPrivatePropertyValue("CreatedDate", now);
            account.SetPrivatePropertyValue("LastModifiedBy", _userId);
            account.SetPrivatePropertyValue("LastModifiedDate", now);

            return account;
        }

        public Session CreateSession()
        {
            throw new NotImplementedException();
        }

        public User CreateUser()
        {
            throw new NotImplementedException();
        }
    }
}
