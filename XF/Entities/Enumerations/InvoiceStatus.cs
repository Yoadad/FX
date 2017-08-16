using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XF.Entities.Enumerations
{
    public enum InvoiceStatus
    {
        Draft = 1,
        InProgress = 2,
        Accepted = 3,
        Refund = 4
    }
}