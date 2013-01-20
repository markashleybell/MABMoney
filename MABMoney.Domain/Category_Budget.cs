using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MABMoney.Domain
{
    [Table("Categories_Budgets")]
    public class Category_Budget : IAuditable
    {
        [Key, Column(Order = 0)]
        [Required]
        public int Budget_BudgetID { get; set; }
        [ForeignKey("Budget_BudgetID")]
        public virtual Budget Budget { get; set; }
        [Key, Column(Order = 1)]
        [Required]
        public int Category_CategoryID { get; set; }
        [ForeignKey("Category_CategoryID")]
        public virtual Category Category { get; set; }
        [Required]
        public decimal Amount { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
