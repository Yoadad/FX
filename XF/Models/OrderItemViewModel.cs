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
            Total = po.Total ?? 0;
            Created = po.Created;
            Status = po.PurchaseOrderStatusId;
        }

        public int PurchaseOrderId { get; set; }
        public DateTime Date { get; set; }
        public DateTime Created { get; set; }
        public decimal Total { get; set; }
        public string PurchaseOrderStatus { get; set; }
        public string ProviderName { get; set; }
        public int Status { get; set; }

    }
}