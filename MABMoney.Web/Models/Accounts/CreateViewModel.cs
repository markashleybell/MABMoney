﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Web.Models.Accounts
{
    public class CreateViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}