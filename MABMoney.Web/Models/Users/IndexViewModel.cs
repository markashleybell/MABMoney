using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MABMoney.Services.DTO;

namespace MABMoney.Web.Models.Users
{
    public class IndexViewModel
    {
        public List<UserDTO> Users { get; set; }
    }
}