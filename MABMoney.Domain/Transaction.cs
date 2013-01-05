using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MABMoney.Domain
{
    public class Transaction
    {
        [Key]
        [Required]
        public int TransactionID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public int Category_CategoryID { get; set; }
        [ForeignKey("Category_CategoryID")]
        public virtual Category Category { get; set; }
    }
}
