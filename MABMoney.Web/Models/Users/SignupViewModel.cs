using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Web.Models.Users
{
    public class SignupViewModel : FormViewModel
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [DisplayName("Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}