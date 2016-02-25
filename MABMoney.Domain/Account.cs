using System.ComponentModel.DataAnnotations;

namespace MABMoney.Domain
{
    public class Account : AuditableEntityBase
    {
        public int AccountID { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal StartingBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public int User_UserID { get; set; }
        public bool Default { get; set; }
        public AccountType Type { get; set; }
        public string TransactionDescriptionHistory { get; set; }
        public int DisplayOrder { get; set; }
        public bool IncludeInNetWorth { get; set; }
    }
}
