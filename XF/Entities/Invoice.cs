namespace XF.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Invoice")]
    public partial class Invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Invoice()
        {
            InvoiceDetails = new HashSet<InvoiceDetail>();
            Payments = new HashSet<Payment>();
            PurchaseOrders = new HashSet<PurchaseOrder>();
        }

        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int ClientId { get; set; }

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

        public int InvoiceStatusId { get; set; }

        public string Comments { get; set; }

        public int PaymentTypeId { get; set; }

        public bool IsDelivery { get; set; }

        [StringLength(1024)]
        public string Address { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? SNAP { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? DeliveryFee { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? InstalationFee { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Refund { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Client Client { get; set; }

        public virtual InvoiceStatu InvoiceStatu { get; set; }

        public virtual PaymentType PaymentType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
