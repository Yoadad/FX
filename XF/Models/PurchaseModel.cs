using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;

namespace XF.Models
{
    public class PurchasesOrdersViewModel
    {
         public IEnumerable<PurchaseOrderStatu> PurchaseOrderStatus { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public float Tax { get; set; }
    }

    public class PurchaseOrderDetailViewModel
    {
        public PurchaseOrderStatu PurchaseStatus { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }
    }
}