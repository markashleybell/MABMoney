using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Models
{
    public class ProfileViewModel
    {
        public int UserID { get; set; }
        public bool IsAdmin { get; set; }
    }
}