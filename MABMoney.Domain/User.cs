using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Domain
{
    public class User : AuditableEntityBase
    {
        [Key]
        [Required]
        public int UserID { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }

        [StringLength(512)]
        public string PasswordResetGUID { get; set; }

        public DateTime? PasswordResetExpiry { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
