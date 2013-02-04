
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MABMoney.Web.Models.Users
{
    public class CreateViewModel
    {
        [Required]
        public string Forename { get; set; }
        [Required]
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