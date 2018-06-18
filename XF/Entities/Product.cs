namespace XF.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            InvoiceDetails = new HashSet<InvoiceDetail>();
            PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
            Stocks = new HashSet<Stock>();
            Categories = new HashSet<Category>();
        }

        public int Id { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Display(Name="Sell Price")]
        [Column(TypeName = "numeric")]
        public decimal SellPrice { get; set; }
        [Display(Name = "Purchase Price")]
        [Column(TypeName = "numeric")]
        public decimal PurchasePrice { get; set; }

        public int Max { get; set; }

        public int Min { get; set; }

        public int ProviderId { get; set; }
        [Display(Name="Display as")]
        [StringLength(255)]
        public string Display { get; set; }

        [NotMapped]
        public string NameCode
        {
            get
            {
                return string.Format("{0} [{1}]", Name, Code);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }

        public virtual Provider Provider { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stock> Stocks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category> Categories { get; set; }
    }
}
