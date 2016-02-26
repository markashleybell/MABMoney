using MABMoney.Services.DTO;
using System.Collections.Generic;

namespace MABMoney.Web.Models.Accounts
{
    public class IndexViewModel
    {
        public List<AccountDTO> Accounts { get; set; }
    }
}