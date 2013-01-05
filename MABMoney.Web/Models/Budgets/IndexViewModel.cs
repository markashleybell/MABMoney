using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MABMoney.Services.DTO;

namespace MABMoney.Web.Models.Budgets
{
    public class IndexViewModel
    {
        public List<BudgetDTO> Budgets { get; set; }
    }
}