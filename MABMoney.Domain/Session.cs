using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Domain
{
    public class Session : AuditableEntityBase
    {
        public int SessionID { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public DateTime Expiry { get; set; }
        [Required]
        public int User_UserID { get; set; }
        [ForeignKey("User_UserID")]
        public virtual User User { get; set; }
    }
}
