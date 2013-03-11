using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class BudgetDTO
    {
        [Required]
        public int BudgetID { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public int Account_AccountID { get; set; }
        public AccountDTO Account { get; set; }

        public List<Category_BudgetDTO> Category_Budgets { get; set; }
    }
}
