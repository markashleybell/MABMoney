using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MABMoney.Services.DTO;
using System.ComponentModel;

namespace MABMoney.Web.Models.Accounts
{
    public class EditViewModel : FormViewModel
    {
        [Required]
        [HiddenInput(DisplayValue = false)]
        public int AccountID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DisplayName("Starting Balance")]
        public decimal StartingBalance { get; set; }
        [Required]
        public AccountTypeDTO Type { get; set; }
        public SelectList AccountTypes { get; set; }
        [Required]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
        [DisplayName("Default Account")]
        public bool Default { get; set; }
    }
}