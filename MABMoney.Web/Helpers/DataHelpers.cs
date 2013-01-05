using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;

namespace MABMoney.Web.Helpers
{
    public static class DataHelpers
    {
        public static IQueryable<SelectListItem> GetAccountSelectOptions(IAccountServices accountServices) 
        {
            return accountServices.All()
                                  .Select(x => new SelectListItem { 
                                      Value = x.AccountID.ToString(),
                                      Text = x.Name
                                  }).AsQueryable();
        }
    }
}