using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Services
{
    public interface IBookingService
    {
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null);
        Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests = 1);
        Task<decimal> CalculateTotalPriceAsync(int roomId, DateTime checkIn, DateTime checkOut);
        Task<Booking> CreateBookingAsync(BookingViewModel model, int userId);
        Task<bool> UpdateBookingStatusAsync(int bookingId, string status);
        Task<List<Booking>> GetUserBookingsAsync(int userId);
        Task<bool> CanUserReviewBookingAsync(int bookingId, int userId);
    }

    public class BookingService : IBookingService
    {
        private readonly HotelDbContext _context;

        public BookingService(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
        {
            var conflictingBookings = await _context.Bookings
                .Where(b => b.RoomId == roomId 
                        && b.Status != "Cancelled" 
                        && (excludeBookingId == null || b.Id != excludeBookingId)
                        && ((b.CheckInDate <= checkIn && b.CheckOutDate > checkIn) ||
                            (b.CheckInDate < checkOut && b.CheckOutDate >= checkOut) ||
                            (b.CheckInDate >= checkIn && b.CheckOutDate <= checkOut)))
                .AnyAsync();

            return !conflictingBookings;
        }        public async Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests = 1)
        {
            // Handle invalid dates - ensure checkout is after checkin
            if (checkOut <= checkIn)
            {
                return new List<Room>(); // Return empty list for invalid date range
            }

            // Get booked room IDs for the given date range
            var bookedRoomIds = await _context.Bookings
                .Where(b => b.Status != "Cancelled" &&
                           ((b.CheckInDate <= checkIn && b.CheckOutDate > checkIn) ||
                            (b.CheckInDate < checkOut && b.CheckOutDate >= checkOut) ||
                            (b.CheckInDate >= checkIn && b.CheckOutDate <= checkOut)))
                .Select(b => b.RoomId)
                .Distinct()
                .ToListAsync();
                
            // Get all available rooms that aren't in the booked list
            // Both Status and IsAvailable should be true, and room must not be booked
            return await _context.Rooms
                .Where(r => r.Status && r.IsAvailable && 
                           r.Capacity >= guests && 
                           !bookedRoomIds.Contains(r.Id))
                .OrderBy(r => r.Price)
                .ToListAsync();
        }

        public async Task<decimal> CalculateTotalPriceAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return 0;

            var nights = (checkOut - checkIn).Days;
            return room.Price * nights;
        }

        public async Task<Booking> CreateBookingAsync(BookingViewModel model, int userId)
        {
            var booking = new Booking
            {
                UserId = userId,
                RoomId = model.RoomId,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                NumberOfGuests = model.NumberOfGuests,
                SpecialRequests = model.SpecialRequests,
                TotalPrice = await CalculateTotalPriceAsync(model.RoomId, model.CheckInDate, model.CheckOutDate),
                Status = "Pending",
                BookingDate = DateTime.Now
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }        public async Task<List<Booking>> GetUserBookingsAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Include(b => b.Payments)
                .Include(b => b.Reviews)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<bool> CanUserReviewBookingAsync(int bookingId, int userId)
        {
            var booking = await _context.Bookings
                .Where(b => b.Id == bookingId && b.UserId == userId && b.Status == "Completed" && b.CheckOutDate < DateTime.Now)
                .FirstOrDefaultAsync();

            if (booking == null) return false;

            var existingReview = await _context.Reviews
                .Where(r => r.BookingId == bookingId && r.UserId == userId)
                .FirstOrDefaultAsync();

            return existingReview == null;
        }
    }
}
