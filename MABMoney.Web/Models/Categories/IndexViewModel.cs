using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MABMoney.Services.DTO;

namespace MABMoney.Web.Models.Categories
{
    public class IndexViewModel
    {
        public List<CategoryDTO> Categories { get; set; }
    }
}