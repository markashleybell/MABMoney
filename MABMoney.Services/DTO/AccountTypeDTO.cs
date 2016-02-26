using System.ComponentModel;

namespace MABMoney.Services.DTO
{
    public enum AccountTypeDTO
    {
        Current,
        Savings,
        [Description("Credit Card")]
        CreditCard,
        Loan
    }
}
