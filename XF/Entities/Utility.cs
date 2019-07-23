namespace XF.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Utility")]
    public partial class Utility
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Type { get; set; }

        public DateTime Date { get; set; }

        [Required]
        [StringLength(128)]
        public string Number { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string Split { get; set; }

        [Column(TypeName = "numeric")]
        public decimal OriginalAmount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal PaidAmount { get; set; }
        [StringLength(256)]
        public string Description { get; set; }

    }
}
