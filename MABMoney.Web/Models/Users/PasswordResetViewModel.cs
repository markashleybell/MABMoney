using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Web.Models.Users
{
    public class PasswordResetViewModel : FormViewModel
    {
        // Hopefully no-one will ever try and enter more than 100 characters as their password...
        // [StringLength(100, MinimumLength = 6)]
        [Required]
        [DisplayName("New Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DisplayName("Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public string GUID { get; set; }
    }
}