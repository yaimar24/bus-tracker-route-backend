using Microsoft.EntityFrameworkCore;
using BusTracker.Models;

namespace BusTracker.Data
{
    public class AppDbContext : DbContext
    {
        // Catálogos
        public DbSet<Company> Companies { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<BusRoute> BusRoutes { get; set; }
        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<BusAssignment> BusAssignments { get; set; }

        // Tracking
        public DbSet<BusCurrentPosition> BusCurrentPositions { get; set; }
        public DbSet<BusPositionHistory> BusPositionsHistory { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Company
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Buses)
                .WithOne(b => b.Company)
                .HasForeignKey(b => b.CompanyId);

            modelBuilder.Entity<Company>()
                .HasMany(c => c.Drivers)
                .WithOne(d => d.Company)
                .HasForeignKey(d => d.CompanyId);

            modelBuilder.Entity<Company>()
                .HasMany(c => c.BusRoutes)
                .WithOne(r => r.Company)
                .HasForeignKey(r => r.CompanyId);

            // Bus
            modelBuilder.Entity<Bus>()
                .HasIndex(b => b.PlateNumber)
                .IsUnique();

            modelBuilder.Entity<Bus>()
                .HasOne(b => b.CurrentPosition)
                .WithOne(cp => cp.Bus)
                .HasForeignKey<BusCurrentPosition>(cp => cp.BusId);

            modelBuilder.Entity<Bus>()
                .HasMany(b => b.Assignments)
                .WithOne(a => a.Bus)
                .HasForeignKey(a => a.BusId);

            modelBuilder.Entity<Bus>()
                .HasMany(b => b.History)
                .WithOne(h => h.Bus)
                .HasForeignKey(h => h.BusId);

            // Driver
            modelBuilder.Entity<Driver>()
                .HasIndex(d => d.LicenseNumber)
                .IsUnique();

            modelBuilder.Entity<Driver>()
                .HasMany(d => d.Assignments)
                .WithOne(a => a.Driver)
                .HasForeignKey(a => a.DriverId);

            // BusRoute
            modelBuilder.Entity<BusRoute>()
                .HasMany(r => r.Stops)
                .WithOne(s => s.BusRoute)
                .HasForeignKey(s => s.BusRouteId);

            modelBuilder.Entity<BusRoute>()
                .HasMany(r => r.Assignments)
                .WithOne(a => a.BusRoute)
                .HasForeignKey(a => a.BusRouteId);

            // RouteStop
            modelBuilder.Entity<RouteStop>()
                .HasIndex(s => new { s.BusRouteId, s.Order })
                .IsUnique();

            // BusAssignment
            modelBuilder.Entity<BusAssignment>()
                .HasOne(a => a.Bus)
                .WithMany(b => b.Assignments)
                .HasForeignKey(a => a.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusAssignment>()
                .HasOne(a => a.Driver)
                .WithMany(d => d.Assignments)
                .HasForeignKey(a => a.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusAssignment>()
                .HasOne(a => a.BusRoute)
                .WithMany(r => r.Assignments)
                .HasForeignKey(a => a.BusRouteId)
                .OnDelete(DeleteBehavior.Restrict);

            // BusCurrentPosition
            modelBuilder.Entity<BusCurrentPosition>()
                .HasKey(cp => cp.BusId); // PK = FK (1:1 con Bus)

            // BusPositionHistory
            modelBuilder.Entity<BusPositionHistory>()
                .HasIndex(h => new { h.BusId, h.Timestamp });
        }
    }
}
