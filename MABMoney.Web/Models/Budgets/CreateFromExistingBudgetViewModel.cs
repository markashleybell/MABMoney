﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace MABMoney.Web.Models.Budgets
{
    public class CreateFromExistingBudgetViewModel : FormViewModel
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [DisplayName("Account")]
        public int Account_AccountID { get; set; }
        public IQueryable<SelectListItem> Accounts { get; set; }

        [DisplayName("Copy from previous")]
        public int Budget_BudgetID { get; set; }
        public IQueryable<SelectListItem> Budgets { get; set; }

        public List<Category_BudgetViewModel> Categories { get; set; }
    }
}