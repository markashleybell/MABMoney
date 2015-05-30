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
        public int CategoryID { get; set; }
        [Required]
        public string Name { get; set; }
        public int Account_AccountID { get; set; }
        public string AccountName { get; set; }
        public CategoryType Type { get; set; }
    }
}
