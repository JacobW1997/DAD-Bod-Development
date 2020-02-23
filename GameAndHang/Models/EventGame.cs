namespace GameAndHang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EventGame
    {
        public int ID { get; set; }

        public int EventID { get; set; }

        public int GameID { get; set; }


        public virtual Event Event { get; set; }

        public virtual Game Game { get; set; }
    }
}
