using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Data
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}