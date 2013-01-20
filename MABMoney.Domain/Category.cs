using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MABMoney.Domain
{
    public class Category : AuditableEntityBase
    {
        [Key]
        [Required]
        public int CategoryID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Account_AccountID { get; set; }
        [ForeignKey("Account_AccountID")]
        public virtual Account Account { get; set; }
        [Required]
        public CategoryType Type { get; set; }
    }
}
