using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestrano.Account
{
    public enum BillStatus
    {
        Unknown = 0,
        Submitted,
        Invoiced,
        Canceled,
    }
}