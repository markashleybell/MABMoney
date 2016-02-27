using MABMoney.Domain.Abstract;
using System;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Domain
{
    public class User : Auditable
    {
        public int UserID { get; set; }
        public Guid GUID { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string PasswordResetGUID { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
    }
}
