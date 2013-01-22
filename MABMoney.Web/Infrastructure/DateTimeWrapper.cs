using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MABMoney.Web.Infrastructure
{
    public class DateTimeWrapper : IDateTimeProvider
    {
        private readonly Func<DateTime> _nowProvider;

        public DateTimeWrapper(Func<DateTime> nowProvider)
        {
            _nowProvider = nowProvider;
        }

        public DateTime Date
        {
            get { return _nowProvider(); }
        }
    }
}