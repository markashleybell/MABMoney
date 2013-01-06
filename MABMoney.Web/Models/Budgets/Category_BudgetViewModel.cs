using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Web.Models.Budgets
{
    public class Category_BudgetViewModel
    {
        [Required]
        public int Budget_BudgetID { get; set; }
        [Required]
        public int Category_CategoryID { get; set; }
        public string Name { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
