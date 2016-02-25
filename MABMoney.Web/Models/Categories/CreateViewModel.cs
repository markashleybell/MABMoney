using MABMoney.Services.DTO;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace MABMoney.Web.Models.Categories
{
    public class CreateViewModel : FormViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [DisplayName("Account")]
        public int Account_AccountID { get; set; }
        public IQueryable<SelectListItem> Accounts { get; set; }
        [Required]
        public CategoryTypeDTO Type { get; set; }
        public SelectList CategoryTypes { get; set; }
    }
}