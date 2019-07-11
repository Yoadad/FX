namespace XF.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Supply")]
    public partial class Supply
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Type { get; set; }

        public DateTime Date { get; set; }

        [StringLength(128)]
        public string Number { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [Column(TypeName = "numeric")]
        public decimal Amount { get; set; }

        public int ProviderId { get; set; }

        public virtual Provider Provider { get; set; }
    }
}
