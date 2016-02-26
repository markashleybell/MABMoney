using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class Category_BudgetDTO
    {
        public int Account_AccountID { get; set; }
        [Required]
        public int Budget_BudgetID { get; set; }
        [Required]
        public int Category_CategoryID { get; set; }
        public string CategoryName { get; set; }
        public CategoryTypeDTO CategoryType { get; set; }
        [Required]
        public decimal Amount { get; set; }

        public decimal Total { get; set; }
    }
}
