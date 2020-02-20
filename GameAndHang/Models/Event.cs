namespace GameAndHang.Models
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
            EventGames = new HashSet<EventGame>();
            EventPlayers = new HashSet<EventPlayer>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(64)]
        public string EventName { get; set; }

        public bool IsPublic { get; set; }

        public DateTime Date { get; set; }

        [Required]
        [StringLength(256)]
        public string EventDescription { get; set; }

        [Required]
        public string EventLocation { get; set; }

        public int PlayerSlotsMin { get; set; }

        public int PlayerSlotsMax { get; set; }

        public string UnsupGames { get; set; }

        public int HostID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventGame> EventGames { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventPlayer> EventPlayers { get; set; }

        public virtual User User { get; set; }
    }
}
