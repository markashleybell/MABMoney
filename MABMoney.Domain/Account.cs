using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MABMoney.Domain
{
    public class Account : AuditableEntityBase
    {
        [Key]
        [Required]
        public int AccountID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal StartingBalance { get; set; }
        [Required]
        public int User_UserID { get; set; }
        [ForeignKey("User_UserID")]
        public virtual User User { get; set; }

        public bool Default { get; set; }

        public AccountType Type { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public string TransactionDescriptionHistory { get; set; }
    }
}
