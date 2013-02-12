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
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public int? Category_CategoryID { get; set; }
        public CategoryDTO Category { get; set; }
        [Required]
        public int Account_AccountID { get; set; }
        public AccountDTO Account { get; set; }

        public string TransferGUID { get; set; }
    }
}
