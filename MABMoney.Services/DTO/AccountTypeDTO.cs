using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Services.DTO
{
    public enum AccountTypeDTO
    {
        Current,
        Savings,
        [Description("Credit Card")]
        CreditCard
    }
}
