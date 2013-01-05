using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class AccountDTO
    {
        [Required]
        public int AccountID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int User_UserID { get; set; }
        public UserDTO User { get; set; }
    }
}
