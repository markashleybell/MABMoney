using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class UserDTO
    {
        [Required]
        public int UserID { get; set; }
        [Required]
        public string Forename { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }

        public virtual List<AccountDTO> Accounts { get; set; }
    }
}
