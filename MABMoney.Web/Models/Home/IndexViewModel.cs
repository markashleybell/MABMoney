using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MABMoney.Services.DTO;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel;
using MABMoney.Web.Models.Budgets;

namespace MABMoney.Web.Models.Home
{
    public class IndexViewModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public AccountDTO Account { get; set; }
        public List<TransactionDTO> Transactions { get; set; }
        
        public IQueryable<SelectListItem> IncomeCategories { get; set; }
        public IQueryable<SelectListItem> ExpenseCategories { get; set; }
        [DisplayName("Account")]
        public int Account_AccountID { get; set; }
        public IQueryable<SelectListItem> Accounts { get; set; }

        public BudgetDTO Budget { get; set; }

        // Transaction form fields
        public TransactionType Type { get; set; }
        [DisplayName("Category")]
        public int? Category_CategoryID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }

        public string Debug { get; set; }
    }
}