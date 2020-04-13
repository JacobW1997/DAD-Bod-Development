namespace GameAndHang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Review
    {
        public string ID { get; set; }

        [StringLength(1000)]
        public string ReviewString { get; set; }

        public string Reviewer_ID { get; set; }

        public string Host_ID { get; set; }
    }
}
