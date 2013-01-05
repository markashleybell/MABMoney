using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Web.Models.Budgets
{
    public class CreateViewModel
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }

        public CreateViewModel()
        {
            Start = DateTime.Now;
            End = Start.AddDays(30);
        }
    }
}