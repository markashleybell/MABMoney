﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MABMoney.Web.Infrastructure
{
    public interface ISiteConfiguration
    {
        string SharedSecret { get; }
    }
}