namespace XF.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ClientNote")]
    public partial class ClientNote
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public int ClientId { get; set; }

        public bool Active { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Client Client { get; set; }
    }
}
