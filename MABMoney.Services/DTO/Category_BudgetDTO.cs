using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class Category_BudgetDTO
    {
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
