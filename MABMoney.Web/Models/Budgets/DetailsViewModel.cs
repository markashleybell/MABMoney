using MABMoney.Services.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Web.Models.Budgets
{
    public class DetailsViewModel
    {
        [Required]
        public int BudgetID { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }

        public List<Category_BudgetDTO> Categories { get; set; }
    }
}