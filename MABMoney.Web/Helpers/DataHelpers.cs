using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;
using MABMoney.Services.DTO;

namespace MABMoney.Web.Helpers
{
    public static class DataHelpers
    {
        public static SelectList GetCategoryTypeSelectOptions()
        {
            return new SelectList(Enum.GetValues(typeof(CategoryTypeDTO)));
        }

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
                                   .Select(x => new SelectListItem
                                   {
                                       Value = x.CategoryID.ToString(),
                                       Text = x.Name
                                   }).AsQueryable();
        }

        public static IQueryable<SelectListItem> GetCategorySelectOptions(ICategoryServices categoryServices, int userId, int accountId)
        {
            return categoryServices.All(userId)
                                   .Where(x => x.Account_AccountID == accountId)
                                   .Select(x => new SelectListItem
                                   {
                                       Value = x.CategoryID.ToString(),
                                       Text = x.Name
                                   }).AsQueryable();
        }

        public static IQueryable<SelectListItem> GetCategorySelectOptions(ICategoryServices categoryServices, int userId, int accountId, CategoryTypeDTO type)
        {
            return categoryServices.All(userId)
                                   .Where(x => x.Type == type && x.Account_AccountID == accountId)
                                   .Select(x => new SelectListItem {
                                       Value = x.CategoryID.ToString(),
                                       Text = x.Name
                                   }).AsQueryable();
        }
    }
}