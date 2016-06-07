namespace MABMoney.Domain.Abstract
{
    public interface IFactory
    {
        Account CreateAccount(int _userId, string name, AccountType type, decimal startingBalance);
        Account CreateAccount(int _userId, string name, AccountType type, decimal startingBalance, bool setDefault);
        Account CreateAccount(int _userId, string name, AccountType type, decimal startingBalance, bool setDefault, int displayOrder);
        Account CreateAccount(int _userId, string name, AccountType type, decimal startingBalance, bool setDefault, int displayOrder, bool includeInNetWorth);

        // Budget CreateBudget();
        // Category_Budget CreateCategory_Budget();
        // Category CreateCategory();
        Session CreateSession();
        // Transaction CreateTransaction();
        User CreateUser();
    }
}
