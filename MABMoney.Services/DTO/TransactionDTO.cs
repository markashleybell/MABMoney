using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class TransactionDTO
    {
        public int TransactionID { get; set; }

        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        public CategoryDTO Category { get; set; }
    }
}
