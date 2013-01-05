using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel;

namespace MABMoney.Web.Models.Transactions
{
    public class EditViewModel
    {
        [Required]
        [HiddenInput(DisplayValue = false)]
        public int TransactionID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        [DisplayName("Category")]
        public int Category_CategoryID { get; set; }
        public IQueryable<SelectListItem> Categories { get; set; }
    }
}