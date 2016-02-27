using MABMoney.Domain.Abstract;
using System;

namespace MABMoney.Domain
{
    public class Transaction : Auditable
    {
        public int TransactionID { get; set; }
        public Guid GUID { get; set; }
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
