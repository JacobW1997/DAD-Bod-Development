namespace DBD_Swim_Tracker_0._2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Result
    {
        public int ID { get; set; }

        public int EVENTID { get; set; }

        public int ATHLETEID { get; set; }

        public int MEETID { get; set; }

        public double TIME { get; set; }

        public virtual Athlete Athlete { get; set; }

        public virtual Event Event { get; set; }

        public virtual Meet Meet { get; set; }
    }
}
