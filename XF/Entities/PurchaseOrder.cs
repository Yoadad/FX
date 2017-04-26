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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseOrder()
        {
            PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
        }

        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int? ProviderId { get; set; }

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
        public decimal? Tax { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Total { get; set; }

        public int PurchaseOrderStatusId { get; set; }

        public string Comments { get; set; }

        public virtual Provider Provider { get; set; }

        public virtual PurchaseOrderStatu PurchaseOrderStatu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    }
}
