namespace GameAndHang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EventPlayer
    {
        public int ID { get; set; }

        public string PlayerID { get; set; }

        public string EventID { get; set; }

        public virtual Event Event { get; set; }

        public virtual User User { get; set; }
    }
}
