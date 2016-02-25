using System;

namespace MABMoney.Domain
{
    public class Budget : AuditableEntityBase
    {
        public int BudgetID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Account_AccountID { get; set; }
    }
}
