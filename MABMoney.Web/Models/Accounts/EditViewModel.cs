﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MABMoney.Services.DTO;

namespace MABMoney.Web.Models.Accounts
{
    public class EditViewModel
    {
        [Required]
        [HiddenInput(DisplayValue = false)]
        public int AccountID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal StartingBalance { get; set; }
        [Required]
        [HiddenInput(DisplayValue = false)]
        public int User_UserID { get; set; }
        [Required]
        public AccountTypeDTO Type { get; set; }
        public SelectList AccountTypes { get; set; }
    }
}