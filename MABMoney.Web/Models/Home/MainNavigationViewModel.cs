using MABMoney.Services.DTO;
using System.Collections.Generic;

namespace MABMoney.Web.Models.Home
{
    public class MainNavigationViewModel
    {
        public int UserID { get; set; }
        public string UserEmail { get; set; }
        public List<AccountDTO> Accounts { get; set; }
        public List<AccountDTO> NetWorthAccounts { get; set; }
        public List<AccountDTO> NonNetWorthAccounts { get; set; }
        public decimal NetWorth { get; set; }
        public string Debug { get; set; }
    }
}