using System;
using HandicapBewerb.Core.Handler;
using HandicapBewerb.DataModels.DbModels;
using Microsoft.EntityFrameworkCore;

namespace HandicapBewerb.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchResult> MatchResults { get; set; }

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
            base.OnModelCreating(modelBuilder);
        }
    }
}
