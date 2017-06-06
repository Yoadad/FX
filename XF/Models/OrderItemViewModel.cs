using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;

namespace XF.Models
{
    public class OrderItemViewModel
    {
        public OrderItemViewModel(PurchaseOrder po)
        {
            PurchaseOrderId = po.Id;
            Date = po.Date;
            Created_Date = po.Created;
            Discount = po.Discount == null ? 0 : po.Discount.Value;
            Subtotal = po.Subtotal == null ? 0 : po.Subtotal.Value;
            Tax = po.Tax == null ? 0 : po.Tax.Value;
            Total = po.Total == null ? 0 : po.Total.Value;
        }

        public int PurchaseOrderId { get; set; }

        public DateTime Date { get; set; }

        
        
        public DateTime Created_Date { get; set; }

        public decimal Discount { get; set; }

        public decimal PercentDiscount { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }
        public string PurchaseOrderStatus { get; set; }

    }
}