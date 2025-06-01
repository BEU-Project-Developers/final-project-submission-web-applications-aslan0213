using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HotelManagementSystem
{
    class AddRoomsProgram
    {
        static async Task Main(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Set up dependency injection
            var serviceProvider = new ServiceCollection()
                .AddDbContext<HotelDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();

                // Check if we already have some rooms
                var existingCount = await dbContext.Rooms.CountAsync();
                Console.WriteLine($"Currently there are {existingCount} rooms in the database.");

                if (existingCount < 5)
                {
                    // Add some sample rooms
                    var rooms = new List<Room>
                    {
                        new Room
                        {
                            RoomNumber = "101",
                            Type = "Deluxe",
                            Name = "Luxury Deluxe Room",
                            Price = 250.00M,
                            Status = true,
                            IsAvailable = true,
                            HotelId = 1,
                            Image = "/img/room/room-1.jpg",
                            Capacity = 2,
                            Description = "Luxury room with king size bed and city view.",
                            Amenities = new List<string> { "WiFi", "Air Conditioning", "Mini Bar", "Flat-screen TV", "Coffee Maker" }
                        },
                        new Room
                        {
                            RoomNumber = "102",
                            Type = "Suite",
                            Name = "Executive Suite",
                            Price = 400.00M,
                            Status = true,
                            IsAvailable = true,
                            HotelId = 1,
                            Image = "/img/room/room-2.jpg",
                            Capacity = 4,
                            Description = "Spacious suite with separate living area and breathtaking sea view.",
                            Amenities = new List<string> { "WiFi", "Air Conditioning", "Mini Bar", "Flat-screen TV", "Coffee Maker", "Balcony", "Jacuzzi" }
                        },
                        new Room
                        {
                            RoomNumber = "103",
                            Type = "Standard",
                            Name = "Comfort Standard Room",
                            Price = 150.00M,
                            Status = true,
                            IsAvailable = true,
                            HotelId = 1,
                            Image = "/img/room/room-3.jpg",
                            Capacity = 2,
                            Description = "Cozy room with all essential amenities for a comfortable stay.",
                            Amenities = new List<string> { "WiFi", "Air Conditioning", "TV", "Coffee Maker" }
                        },
                        new Room
                        {
                            RoomNumber = "201",
                            Type = "Family",
                            Name = "Family Room",
                            Price = 350.00M,
                            Status = true,
                            IsAvailable = true,
                            HotelId = 1,
                            Image = "/img/room/room-4.jpg",
                            Capacity = 5,
                            Description = "Perfect for family stays with extra space and amenities for children.",
                            Amenities = new List<string> { "WiFi", "Air Conditioning", "Mini Bar", "Flat-screen TV", "Coffee Maker", "Children's Play Area" }
                        },
                        new Room
                        {
                            RoomNumber = "202",
                            Type = "Presidential",
                            Name = "Presidential Suite",
                            Price = 800.00M,
                            Status = true,
                            IsAvailable = true,
                            HotelId = 1,
                            Image = "/img/room/room-5.jpg",
                            Capacity = 6,
                            Description = "Our most luxurious accommodation with panoramic views and exclusive services.",
                            Amenities = new List<string> { "WiFi", "Air Conditioning", "Mini Bar", "Flat-screen TV", "Coffee Maker", "Balcony", "Jacuzzi", "Private Butler", "In-room Dining" }
                        }
                    };

                    dbContext.Rooms.AddRange(rooms);
                    await dbContext.SaveChangesAsync();

                    Console.WriteLine($"Added {rooms.Count} new rooms to the database!");
                }
                else
                {
                    // Update existing rooms to ensure they're available
                    var allRooms = await dbContext.Rooms.ToListAsync();
                    int updatedCount = 0;

                    foreach (var room in allRooms)
                    {
                        if (!room.IsAvailable || !room.Status)
                        {
                            room.IsAvailable = true;
                            room.Status = true;
                            updatedCount++;
                        }
                    }

                    if (updatedCount > 0)
                    {
                        await dbContext.SaveChangesAsync();
                        Console.WriteLine($"Updated {updatedCount} rooms to be available.");
                    }
                    else
                    {
                        Console.WriteLine("All rooms are already marked as available.");
                    }
                }

                // Print out all rooms after changes
                Console.WriteLine("\nCurrent rooms in the database:");
                var finalRooms = await dbContext.Rooms.ToListAsync();
                foreach (var room in finalRooms)
                {
                    Console.WriteLine($"Room {room.RoomNumber} (ID: {room.Id}) - Type: {room.Type}, Capacity: {room.Capacity}, " +
                        $"IsAvailable: {room.IsAvailable}, Status: {room.Status}, Price: ${room.Price}");
                }
                
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
