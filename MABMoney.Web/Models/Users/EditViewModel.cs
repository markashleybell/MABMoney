﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MABMoney.Web.Models.Users
{
    public class EditViewModel
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
        public string Password { get; set; }
        [DisplayName("Confirm New Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}