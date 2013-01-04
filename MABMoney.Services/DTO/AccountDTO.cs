using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class AccountDTO
    {
        public int AccountID { get; set; }
        public string Name { get; set; }

        public UserDTO User { get; set; }
    }
}
