using AfghanWheelzz.Models;
using AfghanWheelzz.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace AfghanWheelzz.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Location> Locations { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SeedInitialData(modelBuilder);

            modelBuilder.Entity<Car>()
                .HasOne(c => c.User)
                .WithMany(u => u.Cars)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Car>()
               .HasOne(c => c.Category)
               .WithMany(u => u.Cars)
               .HasForeignKey(c => c.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Car>()
                 .HasOne(c => c.Registration)
                 .WithMany(u => u.Cars)
                 .HasForeignKey(c => c.RegistrationId)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Car>()
                   .HasOne(c => c.Location)
                   .WithMany(u => u.Cars)
                   .HasForeignKey(c => c.LocationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, CategoryName = "Electric" },
                new Category { Id = 2, CategoryName = "Hybrid" },
                new Category { Id = 3, CategoryName = "Gasoline" }
            );

            // Seed locations
            modelBuilder.Entity<Location>().HasData(
                new Location { Id = 1, City = "Jalalabad" },
                new Location { Id = 2, City = "Kabul" },
                new Location { Id = 3, City = "Kunar" }
            );

            // Seed registrations
            modelBuilder.Entity<Registration>().HasData(
                new Registration { Id = 1, RegistrationName = "Registered" },
                new Registration { Id = 2, RegistrationName = "Un-Registered" }
            );
        }
    }
}
