using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Data;
using HotelManagementSystem.Services;

namespace HotelManagementSystem
{
    public class TestRoomSearch
    {
        public static async Task TestSearchFunctionality(HotelDbContext context, IBookingService bookingService)
        {
            Console.WriteLine("=== Testing Room Search Functionality ===");
            
            // Check total rooms in database
            var totalRooms = await context.Rooms.CountAsync();
            Console.WriteLine($"Total rooms in database: {totalRooms}");
            
            // Check available rooms
            var availableRooms = await context.Rooms
                .Where(r => r.Status && r.IsAvailable)
                .ToListAsync();
            Console.WriteLine($"Available rooms (Status=true AND IsAvailable=true): {availableRooms.Count}");
            
            // List available rooms
            foreach (var room in availableRooms)
            {
                Console.WriteLine($"  - Room {room.RoomNumber}: {room.Type}, Capacity: {room.Capacity}, Price: ${room.Price}");
            }
            
            Console.WriteLine("\n=== Testing BookingService.GetAvailableRoomsAsync ===");
            
            // Test search for tomorrow and day after
            var checkIn = DateTime.Today.AddDays(1);
            var checkOut = DateTime.Today.AddDays(2);
            
            Console.WriteLine($"Searching for rooms from {checkIn:yyyy-MM-dd} to {checkOut:yyyy-MM-dd} for 2 guests");
            
            var searchResults = await bookingService.GetAvailableRoomsAsync(checkIn, checkOut, 2);
            Console.WriteLine($"Search results count: {searchResults.Count}");
            
            foreach (var room in searchResults)
            {
                Console.WriteLine($"  - Found: Room {room.RoomNumber} ({room.Type}) - ${room.Price}/night");
            }
            
            if (searchResults.Count > 0)
            {
                Console.WriteLine("\n✅ SUCCESS: Room search is working correctly!");
            }
            else
            {
                Console.WriteLine("\n❌ ISSUE: No rooms found - search may still have issues");
            }
        }
    }
}
