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

        public static SelectList GetAccountTypeSelectOptions()
        {
            return new SelectList(Enum.GetValues(typeof(AccountTypeDTO)));
        }

        public static IQueryable<SelectListItem> GetAccountSelectOptions(IAccountServices accountServices, bool showBalances = false) 
        {
            return accountServices.All().Select(x => new SelectListItem { 
                Value = x.AccountID.ToString(),
                Text = x.Name + ((showBalances) ? " / " + x.CurrentBalance : "")
            }).AsQueryable();
        }

        public static IQueryable<SelectListItem> GetCategorySelectOptions(ICategoryServices categoryServices)
        {
            return categoryServices.All().Select(x => new SelectListItem {
                Value = x.CategoryID.ToString(),
                Text = x.Name
            }).AsQueryable();
        }

        public static IQueryable<SelectListItem> GetCategorySelectOptions(ICategoryServices categoryServices, int accountId)
        {
            return categoryServices.All().Where(x => x.Account_AccountID == accountId).Select(x => new SelectListItem {
                Value = x.CategoryID.ToString(),
                Text = x.Name
            }).AsQueryable();
        }

        public static IQueryable<SelectListItem> GetCategorySelectOptions(ICategoryServices categoryServices, int accountId, CategoryTypeDTO type)
        {
            return categoryServices.All().Where(x => x.Type == type && x.Account_AccountID == accountId).Select(x => new SelectListItem {
                Value = x.CategoryID.ToString(),
                Text = x.Name
            }).AsQueryable();
        }
    }
}