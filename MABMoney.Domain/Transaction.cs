using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Domain
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        public virtual Category Category { get; set; }
    }
}
