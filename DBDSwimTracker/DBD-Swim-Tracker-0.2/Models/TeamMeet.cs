namespace DBD_Swim_Tracker_0._2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TeamMeet
    {
        public int ID { get; set; }

        public int TEAMID { get; set; }

        public int MEETID { get; set; }

        public virtual Meet Meet { get; set; }

        public virtual Team Team { get; set; }
    }
}
