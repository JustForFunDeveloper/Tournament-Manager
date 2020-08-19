using System;
using TournamentManager.Core.Handler;
using TournamentManager.DataModels.DbModels;
using Microsoft.EntityFrameworkCore;

namespace TournamentManager.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Round> Rounds { get; set; }

        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchResult> MatchResults { get; set; }
        public DbSet<UserMatch> UserMatches { get; set; }

        public DbSet<UserTeamMatch> UserTeamMatches { get; set; }
        public DbSet<TeamMatch> TeamMatches { get; set; }
        public DbSet<SoloTeamMatchResult> SoloTeamMatchResults { get; set; }
        public DbSet<TeamMatchResult> TeamMatchResults { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseSqlite("Data Source=local.db");
                base.OnConfiguring(optionsBuilder);
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("UpdateLocalTvShows: " + ex, LogLevel.Error);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name).IsUnique();

            modelBuilder.Entity<UserMatch>()
                .HasKey(um => new { um.UserId, um.MatchId });
            modelBuilder.Entity<UserMatch>()
                .HasOne(um => um.User)
                .WithMany(u => u.UserMatches)
                .HasForeignKey(um => um.UserId);
            modelBuilder.Entity<UserMatch>()
                .HasOne(um => um.Match)
                .WithMany(m => m.UserMatches)
                .HasForeignKey(um => um.MatchId);

            modelBuilder.Entity<UserTeamMatch>()
                .HasKey(um => new { um.UserId, um.TeamMatchId });
            modelBuilder.Entity<UserTeamMatch>()
                .HasOne(um => um.User)
                .WithMany(u => u.UserTeamMatches)
                .HasForeignKey(um => um.UserId);
            modelBuilder.Entity<UserTeamMatch>()
                .HasOne(um => um.TeamMatch)
                .WithMany(m => m.UserTeamMatches)
                .HasForeignKey(um => um.TeamMatchId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
