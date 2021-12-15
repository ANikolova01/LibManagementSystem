using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext() { }
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BranchHours> BranchHours { get; set; }
        public virtual DbSet<CheckoutHistory> CheckoutHistories { get; set; }
        public virtual DbSet<Checkout> Checkouts { get; set; }
        public virtual DbSet<LibraryBranch> LibraryBranches { get; set; }
        public virtual DbSet<LibraryCard> LibraryCards { get; set; }
        public virtual DbSet<Reservation> Reservation { get; set; }
        public virtual DbSet<Patron> Patrons { get; set; }
        public virtual DbSet<AvailabilityStatus> Statuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SeedInitialAssetStatuses(modelBuilder);
            LinkBranchHours(modelBuilder);
            LinkPatrons(modelBuilder);
        }

        public static void LinkBranchHours(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BranchHours>().HasOne(branchH => branchH.Branch);
        }

        public static void LinkPatrons(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patron>().HasOne(patron => patron.HomeLibraryBranch);
        }

        private static void SeedInitialAssetStatuses(ModelBuilder modelBuilder)
        {
            var statusLost = new AvailabilityStatus
            {
                Id = 1,
                Name = BookStatus.Lost,
                Description = "The item is lost."
            };
            var statusGood = new AvailabilityStatus
            {
                Id = 2,
                Name = BookStatus.GoodCondition,
                Description = "The item is in good condition."
            };

            var statusUnknown = new AvailabilityStatus
            {
                Id = 3,
                Name = BookStatus.Unknown,
                Description = "The item is in unknown whereabouts and condition."
            };

            var statusDestroyed = new AvailabilityStatus
            {
                Id = 4,
                Name = BookStatus.Destroyed,
                Description = "The item has been destroyed."
            };
            var defaultStatuses = new List<AvailabilityStatus> {
                statusLost,
                statusGood,
                statusUnknown,
                statusDestroyed
             };

            // Seeding initial Asset Statuses
            modelBuilder.Entity<AvailabilityStatus>().HasData(defaultStatuses);
        }
    }
}

