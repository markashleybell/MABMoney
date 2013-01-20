using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MABMoney.Domain
{
    public class Budget : AuditableEntityBase
    {
        [Key]
        [Required]
        public int BudgetID { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public int Account_AccountID { get; set; }
        [ForeignKey("Account_AccountID")]
        public virtual Account Account { get; set; }

        public virtual ICollection<Category_Budget> Category_Budgets { get; set; }
    }
}
