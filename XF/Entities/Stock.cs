namespace XF.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Stock")]
    public partial class Stock
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int LocationId { get; set; }

        [Column("Stock")]
        public int StockQuantity { get; set; }

        public virtual Location Location { get; set; }

        public virtual Product Product { get; set; }
    }
}
