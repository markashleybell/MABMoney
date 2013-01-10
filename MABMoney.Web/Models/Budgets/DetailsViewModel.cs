using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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

        public List<Category_BudgetViewModel> Categories { get; set; }
    }
}