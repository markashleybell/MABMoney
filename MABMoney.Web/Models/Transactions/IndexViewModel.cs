using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MABMoney.Services.DTO;
using System.Web.Mvc;

namespace MABMoney.Web.Models.Transactions
{
    public class IndexViewModel
    {
        public int? AccountID { get; set; }
        public int? CategoryID { get; set; }
        public List<TransactionDTO> Transactions { get; set; }
        public IEnumerable<SelectListItem> Accounts { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}