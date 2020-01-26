namespace DBD_SwimTracker.DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using DBD_SwimTracker.Models;

    public partial class SwimTrackerContext : DbContext
    {
        public SwimTrackerContext()
            : base("name=SwimTrackerContext")
        {
        }

        public virtual DbSet<Athlete> Athletes { get; set; }
        public virtual DbSet<Coach> Coaches { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Meet> Meets { get; set; }
        public virtual DbSet<Result> Results { get; set; }
        public virtual DbSet<TeamMeet> TeamMeets { get; set; }
        public virtual DbSet<TeamRoster> TeamRosters { get; set; }
        public virtual DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Athlete>()
                .HasMany(e => e.Results)
                .WithRequired(e => e.Athlete)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Athlete>()
                .HasMany(e => e.TeamRosters)
                .WithRequired(e => e.Athlete)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Coach>()
                .HasMany(e => e.Teams)
                .WithRequired(e => e.Coach)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Results)
                .WithRequired(e => e.Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Meet>()
                .HasMany(e => e.Events)
                .WithRequired(e => e.Meet)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Meet>()
                .HasMany(e => e.TeamMeets)
                .WithRequired(e => e.Meet)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.TeamMeets)
                .WithRequired(e => e.Team)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.TeamRosters)
                .WithRequired(e => e.Team)
                .WillCascadeOnDelete(false);
        }
    }
}
