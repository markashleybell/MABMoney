
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MABMoney.Web.Models.Users
{
    public class EditViewModel
    {
        [Required]
        [HiddenInput(DisplayValue = false)]
        public int UserID { get; set; }
        [Required]
        public string Forename { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
    }
}