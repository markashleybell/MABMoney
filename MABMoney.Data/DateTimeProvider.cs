using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Data
{
    public class DateTimeProvider : IDateTimeProvider
    {
        private readonly Func<DateTime> _nowProvider;

        public DateTimeProvider(Func<DateTime> nowProvider)
        {
            _nowProvider = nowProvider;
        }

        public DateTime Now
        {
            get { return _nowProvider(); }
        }
    }
}