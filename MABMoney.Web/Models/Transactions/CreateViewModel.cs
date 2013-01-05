using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Web.Models.Transactions
{
    public class CreateViewModel
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }

        public CreateViewModel()
        {
            Date = DateTime.Now;
        }
    }
}