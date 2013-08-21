using MABMoney.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Models.Home
{
    public class MainNavigationViewModel
    {
        public ProfileViewModel Profile { get; set; }
        public List<AccountDTO> Accounts { get; set; }
        public decimal NetWorth { get; set; }
        public string Debug { get; set; }
    }
}