using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Models;

namespace HotelManagementSystem.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Role).HasDefaultValue("Guest");
                entity.HasIndex(e => e.Email).IsUnique();
            });            // Hotel configurations
            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).IsRequired();
                entity.HasMany(h => h.Rooms)
                    .WithOne()
                    .HasForeignKey(r => r.HotelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });// Room configurations
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");                entity.Property(e => e.Amenities).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                ).Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));
            });// Booking configurations
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasDefaultValue("Pending");
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Bookings)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Room)
                    .WithMany(e => e.Bookings)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);
            });            // Payment configurations
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PaymentMethod).HasDefaultValue("Credit Card");
                entity.Property(e => e.Status).HasDefaultValue("Pending");
                entity.HasOne(e => e.Booking)
                    .WithMany(e => e.Payments)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });            // Review configurations
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Rating).IsRequired();
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Reviews)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.Room)
                    .WithMany(e => e.Reviews)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.Booking)
                    .WithMany(e => e.Reviews)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Hotels
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel { Id = 1, Name = "LuxeVoyage Hotel", Address = "FR99+HPM, Jafar Jabbarli, Baku, Azerbaijan", Description = "Luxury hotel in the heart of Baku", PhoneNumber = "+994 50 598 69 33", Email = "info.luxevoyage@gmail.com" }
            );            // Seed Admin User
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    FullName = "Admin User", 
                    Email = "admin@luxevoyage.com", 
                    Password = "JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=", // SHA256 hash of "admin123"
                    Role = "Admin",
                    PhoneNumber = "+994 50 598 69 33",
                    Address = "Baku, Azerbaijan"
                }
            );// Seed Rooms
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, RoomNumber = "101", Type = "Deluxe", Name = "Deluxe Ocean View", Price = 150.00m, Status = true, IsAvailable = true, HotelId = 1, Image = "room1.jpg", Capacity = 2, Description = "Luxurious deluxe room with ocean view" },
                new Room { Id = 2, RoomNumber = "102", Type = "Suite", Name = "Executive Suite", Price = 250.00m, Status = true, IsAvailable = true, HotelId = 1, Image = "room2.jpg", Capacity = 4, Description = "Spacious executive suite with premium amenities" },
                new Room { Id = 3, RoomNumber = "103", Type = "Standard", Name = "Standard Comfort", Price = 100.00m, Status = true, IsAvailable = true, HotelId = 1, Image = "room3.jpg", Capacity = 2, Description = "Comfortable standard room with essential amenities" },
                new Room { Id = 4, RoomNumber = "201", Type = "Premium", Name = "Premium City View", Price = 200.00m, Status = true, IsAvailable = true, HotelId = 1, Image = "room4.jpg", Capacity = 3, Description = "Premium room with stunning city views" },
                new Room { Id = 5, RoomNumber = "202", Type = "Family", Name = "Family Paradise", Price = 300.00m, Status = true, IsAvailable = true, HotelId = 1, Image = "room5.jpg", Capacity = 6, Description = "Perfect family room with multiple beds and entertainment" }
            );
        }
    }
}
