namespace GameAndHang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Relationship")]
    public partial class Relationship
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Key]
        [Column(Order = 0)]
        public string UserFirstID { get; set; }

        [Key]
        [Column(Order = 1)]
        public string UserSecondID { get; set; }

        public int Type { get; set; }

        public virtual RelationshipType RelationshipType { get; set; }
    }
}
