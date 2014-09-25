﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Caching
{
    public enum CachingDependency
    {
        Category,
        Transaction,
        Account,
        User,
        Budget,
        All
    }
}