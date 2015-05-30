using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MABMoney.Domain
{
    public class Transaction : AuditableEntityBase
    {
        public int TransactionID { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public decimal Amount { get; set; }
        public int? Category_CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int Account_AccountID { get; set; }
        public string AccountName { get; set; }
        public string TransferGUID { get; set; }
    }
}
