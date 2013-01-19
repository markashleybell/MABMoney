using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel;
using MABMoney.Services.DTO;

namespace MABMoney.Web.Models.Categories
{
    public class EditViewModel
    {
        [Required]
        [HiddenInput(DisplayValue = false)]
        public int CategoryID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DisplayName("Account")]
        public int Account_AccountID { get; set; }
        public IQueryable<SelectListItem> Accounts { get; set; }
        [Required]
        public CategoryTypeDTO Type { get; set; }
    }
}