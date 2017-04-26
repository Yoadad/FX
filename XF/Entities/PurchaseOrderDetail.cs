﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace XF.Entities
{
    [Table("PurchaseOrderDetail")]
    public partial class PurchaseOrderDetail
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        [Column(TypeName = "numeric")]
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public virtual PurchaseOrder PurchaseOrder { get; set; }

        public virtual Product Product { get; set; }

    }
}