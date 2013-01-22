using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Infrastructure
{
    public interface IDateTimeProvider
    {
        DateTime Date { get; }
    }
}