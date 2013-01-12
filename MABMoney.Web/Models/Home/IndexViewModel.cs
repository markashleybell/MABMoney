using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MABMoney.Services.DTO;

namespace MABMoney.Web.Models.Home
{
    public class IndexViewModel
    {
        public AccountDTO Account { get; set; }
        public List<TransactionDTO> Transactions { get; set; }
    }
}