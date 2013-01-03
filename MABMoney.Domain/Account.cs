using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Domain
{
    public class Account
    {
        [Key]
        public int AccountID { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual User User { get; set; }
    }
}
