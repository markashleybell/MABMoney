using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class TransactionDTO
    {
        [Required]
        public int TransactionID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public int Category_CategoryID { get; set; }
        public CategoryDTO Category { get; set; }
    }
}
