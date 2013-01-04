using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class CategoryDTO
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }

        public AccountDTO Account { get; set; }
    }
}
