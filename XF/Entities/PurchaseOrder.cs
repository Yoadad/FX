namespace XF.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PurchaseOrder")]
    public partial class PurchaseOrder
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int ProviderId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public DateTime Created { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Discount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? PercentDiscount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Subtotal { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Total { get; set; }

        public int PurchaseOrderStatusId { get; set; }

        public virtual Provider Provider { get; set; }

        public virtual PurchaseOrderStatu PurchaseOrderStatu { get; set; }
    }
}
