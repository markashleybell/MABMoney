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
        public static IQueryable<SelectListItem> GetAccountSelectOptions(IAccountServices accountServices, int userId) 
        {
            return accountServices.All(userId)
                                  .Select(x => new SelectListItem { 
                                      Value = x.AccountID.ToString(),
                                      Text = x.Name
                                  }).AsQueryable();
        }

        public static IQueryable<SelectListItem> GetCategorySelectOptions(ICategoryServices categoryServices, int userId)
        {
            return categoryServices.All(userId)
                                   .Select(x => new SelectListItem {
                                       Value = x.CategoryID.ToString(),
                                       Text = x.Name
                                   }).AsQueryable();
        }
    }
}