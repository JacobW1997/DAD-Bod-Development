namespace GameAndHang.DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using GameAndHang.Models;

    public partial class GnHContext : DbContext
    {
        public GnHContext()
            : base("name=GnHContext")
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<EventGame> EventGames { get; set; }
        public virtual DbSet<EventPlayer> EventPlayers { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Relationship> Relationships { get; set; }
        public virtual DbSet<RelationshipType> RelationshipTypes { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<APIEventGame> APIEventGames { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasOptional(e => e.User)
                .WithRequired(e => e.AspNetUser)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Event>()
                .HasMany(e => e.APIEventGames)
                .WithRequired(e => e.Event)
                .WillCascadeOnDelete(false);

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

            modelBuilder.Entity<RelationshipType>()
                .HasMany(e => e.Relationships)
                .WithRequired(e => e.RelationshipType)
                .HasForeignKey(e => e.Type);

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

        //public System.Data.Entity.DbSet<GameAndHang.Models.Review> Reviews { get; set; }
    }
}
