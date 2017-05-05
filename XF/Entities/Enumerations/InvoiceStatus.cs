using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XF.Entities.Enumerations
{
    public enum InvoiceStatus
    {
        Quotation = 1,
        Draft = 2,
        InProgress = 3,
        Accepted = 4
    }
}