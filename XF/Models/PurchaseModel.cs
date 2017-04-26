using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;

namespace XF.Models
{

    public class PurchaseItemViewmodel : PurchaseOrder
    {
        public PurchaseItemViewmodel()
        {

        }

        public PurchaseItemViewmodel(PurchaseOrder p)
        {
            this.Comments = p.Comments;
            this.Created = p.Created;
            this.Date = p.Date;
            this.Discount = p.Discount;
            this.Id = p.Id;
            this.PercentDiscount = p.PercentDiscount;
            this.PurchaseOrderStatusId = p.PurchaseOrderStatusId;
            this.Subtotal = p.Subtotal;
            this.Tax = p.Tax;
            this.Total = p.Total;

        }

        public PurchaseOrderStatu PurchaseOrderStatus { get; set; }
    }

    public class PurchasesOrdersViewModel
    {
         public IEnumerable<PurchaseOrderDetail> PurchasesOrderDetails { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public float Tax { get; set; }
    }

    public class PurchaseOrderDetailViewModel
    {
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}