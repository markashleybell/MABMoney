using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class BudgetDTO
    {
        public int BudgetID { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public virtual List<CategoryDTO> Categories { get; set; }
    }
}
