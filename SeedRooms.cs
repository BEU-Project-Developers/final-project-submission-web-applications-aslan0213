using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Data;
using HotelManagementSystem.Models;

namespace HotelManagementSystem
{
    public class SeedRooms
    {
        public static async Task SeedRoomsData(HotelDbContext context)
        {
            // Check if rooms already exist
            if (await context.Rooms.AnyAsync())
            {
                Console.WriteLine("Rooms already exist in database. Skipping seed.");
                return;
            }

            var rooms = new List<Room>
            {                new Room
                {
                    RoomNumber = "101",
                    Type = "Deluxe",
                    Name = "Deluxe City View",
                    Capacity = 2,
                    Price = 150.00m,
                    IsAvailable = true,
                    Status = true,
                    HotelId = 1,
                    Amenities = new List<string> { "WiFi", "TV", "Air Conditioning", "Mini Bar" },
                    Description = "Luxurious deluxe room with city view"
                },
                new Room
                {
                    RoomNumber = "102",
                    Type = "Suite",
                    Name = "Executive Suite",
                    Capacity = 4,
                    Price = 300.00m,
                    IsAvailable = true,
                    Status = true,
                    HotelId = 1,
                    Amenities = new List<string> { "WiFi", "TV", "Air Conditioning", "Mini Bar", "Jacuzzi", "Balcony" },
                    Description = "Spacious suite with separate living area"
                },
                new Room
                {
                    RoomNumber = "103",
                    Type = "Standard",
                    Name = "Standard Room",
                    Capacity = 2,
                    Price = 100.00m,
                    IsAvailable = true,
                    Status = true,
                    HotelId = 1,
                    Amenities = new List<string> { "WiFi", "TV", "Air Conditioning" },
                    Description = "Comfortable standard room"
                },
                new Room
                {
                    RoomNumber = "201",
                    Type = "Family",
                    Name = "Family Room",
                    Capacity = 6,
                    Price = 250.00m,
                    IsAvailable = true,
                    Status = true,
                    HotelId = 1,
                    Amenities = new List<string> { "WiFi", "TV", "Air Conditioning", "Mini Fridge", "Bunk Beds" },
                    Description = "Large family room with multiple beds"
                },
                new Room
                {
                    RoomNumber = "301",
                    Type = "Presidential",
                    Name = "Presidential Suite",
                    Capacity = 4,
                    Price = 500.00m,
                    IsAvailable = true,
                    Status = true,
                    HotelId = 1,
                    Amenities = new List<string> { "WiFi", "TV", "Air Conditioning", "Mini Bar", "Jacuzzi", "Balcony", "Room Service" },
                    Description = "Luxury presidential suite with premium amenities"
                }
            };

            await context.Rooms.AddRangeAsync(rooms);
            await context.SaveChangesAsync();
            
            Console.WriteLine($"Successfully added {rooms.Count} rooms to the database.");
        }
    }
}
