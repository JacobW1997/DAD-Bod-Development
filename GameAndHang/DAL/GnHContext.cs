namespace GameAndHang.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GnHContext : DbContext
    {
        public GnHContext()
            : base("name=GnHContext")
        {
        }

        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<EventGame> EventGames { get; set; }
        public virtual DbSet<EventPlayer> EventPlayers { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.CredentialsID);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventGames)
                .WithRequired(e => e.Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventPlayers)
                .WithRequired(e => e.Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Game>()
                .HasMany(e => e.EventGames)
                .WithRequired(e => e.Game)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.EventPlayers)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.PlayerID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Events)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.HostID);
        }
    }
}
