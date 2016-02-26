using MABMoney.Domain.Abstract;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Domain
{
    public class Category : Auditable
    {
        public int CategoryID { get; set; }
        [Required]
        public string Name { get; set; }
        public int Account_AccountID { get; set; }
        public string AccountName { get; set; }
        public CategoryType Type { get; set; }
    }
}
