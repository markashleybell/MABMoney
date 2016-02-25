using System;
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
        public string Note { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public int? Category_CategoryID { get; set; }
        public string CategoryName { get; set; }
        [Required]
        public int Account_AccountID { get; set; }
        public string AccountName { get; set; }

        public string TransferGUID { get; set; }
    }
}
