namespace XF.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InvoiceDetail")]
    public partial class InvoiceDetail
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public int ProductId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public int InOrder { get; set; }

        public string Note { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual Product Product { get; set; }
    }
}
