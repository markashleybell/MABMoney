﻿using MABMoney.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Models.Home
{
    public class MoreTransactionsViewModel
    {
        public DateTime From { get; set; }
        public IEnumerable<TransactionDTO> Transactions { get; set; }
    }
}