namespace GameAndHang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class APIEventGame
    {
        public int ID { get; set; }

        [Required]
        [StringLength(128)]
        public string EventID { get; set; }

        [StringLength(16)]
        public string GameID { get; set; }

        [StringLength(32)]
        public string GameName { get; set; }

        public virtual Event Event { get; set; }
    }
}
