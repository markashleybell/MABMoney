using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Models.Users
{
    public class PasswordResetRequestViewModel : FormViewModel
    {
        public string Email { get; set; }
    }
}