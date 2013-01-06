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
        public virtual BudgetDTO Budget { get; set; }
        [Required]
        public int Category_CategoryID { get; set; }
        public virtual CategoryDTO Category { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
