using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MABMoney.Web.Models.Transactions
{
    public class ImportViewModel
    {
        public HttpPostedFileBase File { get; set; }
        public int RecordsImported { get; set; }
        public int Account_AccountID { get; set; }
        public IQueryable<SelectListItem> Accounts { get; set; }
    }
}