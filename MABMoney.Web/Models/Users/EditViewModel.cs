using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Web.Models.Users
{
    public class EditViewModel : FormViewModel
    {
        [Required]
        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public int UserID { get; set; }
        [Required]
        public string Forename { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        [DisplayName("New Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DisplayName("Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}