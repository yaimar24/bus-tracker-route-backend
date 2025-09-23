using Microsoft.EntityFrameworkCore;
using BusTracker.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BusTracker.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<BusPosition> BusPositions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BusPosition>().HasKey(b => b.Id);
            modelBuilder.Entity<BusPosition>().HasIndex(b => b.BusId);
            modelBuilder.Entity<BusPosition>().HasIndex(b => b.Route);
            base.OnModelCreating(modelBuilder);
        }
    }
}
