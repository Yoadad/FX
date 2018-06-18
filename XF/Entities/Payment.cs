namespace XF.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Payment")]
    public partial class Payment
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Column(TypeName = "numeric")]
        public decimal Amount { get; set; }

        public int PaymentOptionId { get; set; }

        public int InvoiceId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal Balance { get; set; }

        public bool HasFee { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }
        [NotMapped]
        [StringLength(128)]
        public string UserName { get; set; }


        public virtual Invoice Invoice { get; set; }

        public virtual PaymentOption PaymentOption { get; set; }
    }
}
