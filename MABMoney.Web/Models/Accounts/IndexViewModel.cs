using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MABMoney.Services.DTO;

namespace MABMoney.Web.Models.Accounts
{
    public class IndexViewModel
    {
        public List<AccountDTO> Accounts { get; set; }
    }
}