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
        public AccountDTO Account { get; set; }
        public List<TransactionDTO> Transactions { get; set; }

        public TransactionType Type { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [DisplayName("Category")]
        public int? Category_CategoryID { get; set; }
        public IQueryable<SelectListItem> Categories { get; set; }
        [DisplayName("Account")]
        public int Account_AccountID { get; set; }
        public IQueryable<SelectListItem> Accounts { get; set; }

        public BudgetDTO Budget { get; set; }

        public decimal NetWorth { get; set; }

        public IndexViewModel()
        {
            Date = DateTime.Now;
        }
    }

    public enum TransactionType
    {
        Income,
        Expense
    }
}