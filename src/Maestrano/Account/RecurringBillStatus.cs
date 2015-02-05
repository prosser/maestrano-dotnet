using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestrano.Account
{
    public enum RecurringBillStatus
    {
        Unknown = 0,
        Submitted,
        Active,
        Expired,
        Canceled,
    }
}