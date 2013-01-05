using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MABMoney.Services.DTO;

namespace MABMoney.Web.Models.Transactions
{
    public class IndexViewModel
    {
        public List<TransactionDTO> Transactions { get; set; }
    }
}