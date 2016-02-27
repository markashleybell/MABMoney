using MABMoney.Domain.Abstract;
using System;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Domain
{
    public class Session : Auditable
    {
        public int SessionID { get; set; }
        public Guid GUID { get; set; }
        [Required]
        public string Key { get; set; }
        public DateTime Expiry { get; set; }
        public int User_UserID { get; set; }
    }
}
