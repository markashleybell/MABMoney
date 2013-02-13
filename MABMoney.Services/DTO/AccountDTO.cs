using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    [Serializable]
    public class AccountDTO
    {
        [Required]
        public int AccountID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal StartingBalance { get; set; }

        public decimal CurrentBalance { get; set; }

        public bool Default { get; set; }

        public AccountTypeDTO Type { get; set; }

        public List<string> TransactionDescriptionHistory { get; set; }
    }
}
