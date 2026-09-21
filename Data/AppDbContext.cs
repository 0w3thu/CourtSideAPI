using CourtSideAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourtSideAPI.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<RefreshToken> RefreshTokens {  get; set; }
             

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Team>()
                .HasOne(t => t.Coach)
                .WithMany(c => c.Teams)
                .HasForeignKey(t => t.CoachId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Team>()
              .Property(t => t.Division)
              .HasConversion<string>();

        }
    }
}
