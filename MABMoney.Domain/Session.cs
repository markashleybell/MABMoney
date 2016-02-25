using System;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Domain
{
    public class Session : AuditableEntityBase
    {
        public int SessionID { get; set; }
        [Required]
        public string Key { get; set; }
        public DateTime Expiry { get; set; }
        public int User_UserID { get; set; }
    }
}
