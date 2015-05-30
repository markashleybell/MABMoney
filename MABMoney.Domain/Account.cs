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
    }
}
