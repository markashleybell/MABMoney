using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Domain
{
    public class User : AuditableEntityBase
    {
        public int UserID { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string PasswordResetGUID { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
    }
}
