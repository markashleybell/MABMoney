using System;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class SessionDTO
    {
        public int SessionID { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public DateTime Expiry { get; set; }
        [Required]
        public int User_UserID { get; set; }
    }
}
