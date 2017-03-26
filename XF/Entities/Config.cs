namespace XF.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Config")]
    public partial class Config
    {
        [Key]
        [StringLength(256)]
        public string Key { get; set; }

        [Required]
        [StringLength(256)]
        public string Value { get; set; }
    }
}
