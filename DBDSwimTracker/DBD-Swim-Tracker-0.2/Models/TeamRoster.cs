namespace DBD_Swim_Tracker_0._2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TeamRoster
    {
        public int ID { get; set; }

        public int TEAMID { get; set; }

        public int ATHLETEID { get; set; }

        public virtual Athlete Athlete { get; set; }

        public virtual Team Team { get; set; }
    }
}
