namespace DBD_SwimTracker.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Event
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Event()
        {
            Results = new HashSet<Result>();
        }

        public int ID { get; set; }

        public int DISTANCE { get; set; }

        [Required]
        public string STROKE { get; set; }

        public string AGEGROUP { get; set; }

        [Required]
        public string GENDER { get; set; }

        public int MEETID { get; set; }

        public virtual Meet Meet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Result> Results { get; set; }
    }
}
