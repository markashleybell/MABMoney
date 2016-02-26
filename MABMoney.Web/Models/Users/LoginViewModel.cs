using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Web.Models.Users
{
    public class LoginViewModel : FormViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }
}