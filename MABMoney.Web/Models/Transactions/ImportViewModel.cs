using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Models.Transactions
{
    public class ImportViewModel
    {
        public HttpPostedFileBase File { get; set; }
        public bool DoImport { get; set; }
        public bool Imported { get; set; }
    }
}