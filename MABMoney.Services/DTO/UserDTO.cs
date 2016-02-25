using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class UserDTO
    {
        [Required]
        public int UserID { get; set; }
        
        public string Forename { get; set; }
        
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordResetGUID { get; set; }

        public DateTime? PasswordResetExpiry { get; set; }

        public List<AccountDTO> Accounts { get; set; }
    }
}
