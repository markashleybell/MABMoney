using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MABMoney.Web.Models.Transactions
{
    public class ImportViewModel : FormViewModel
    {
        public HttpPostedFileBase File { get; set; }
        public int RecordsImported { get; set; }
        [DisplayName("Account")]
        public int Account_AccountID { get; set; }
        public IQueryable<SelectListItem> Accounts { get; set; }
    }
}