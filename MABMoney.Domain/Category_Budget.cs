using MABMoney.Domain.Abstract;
using System;

namespace MABMoney.Domain
{
    public class Category_Budget : Auditable
    {
        public int Budget_BudgetID { get; set; }
        public int Category_CategoryID { get; set; }
        public Guid GUID { get; set; }
        public string CategoryName { get; set; }
        public CategoryType CategoryType { get; set; }
        public decimal Amount { get; set; }
    }
}
