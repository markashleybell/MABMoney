using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace MABMoney.Web.Models.Transactions
{
    public class EditViewModel : FormViewModel
    {
        [Required]
        [HiddenInput(DisplayValue = false)]
        public int TransactionID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [DisplayName("Category")]
        public int? Category_CategoryID { get; set; }
        public IQueryable<SelectListItem> Categories { get; set; }
        [DisplayName("Account")]
        public int Account_AccountID { get; set; }
        public IQueryable<SelectListItem> Accounts { get; set; }
        public string TransferGUID { get; set; }
    }
}