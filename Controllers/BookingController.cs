using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using HotelManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelManagementSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly HotelDbContext _context;
        private readonly IBookingService _bookingService;
        private readonly IPaymentService _paymentService;

        public BookingController(HotelDbContext context, IBookingService bookingService, IPaymentService paymentService)
        {
            _context = context;
            _bookingService = bookingService;
            _paymentService = paymentService;
        }

        public async Task<IActionResult> Search(DateTime? checkIn, DateTime? checkOut, int guests = 1, string roomType = "")
        {
            var model = new SearchViewModel
            {
                CheckInDate = checkIn ?? DateTime.Today.AddDays(1),
                CheckOutDate = checkOut ?? DateTime.Today.AddDays(2),
                Guests = guests,
                RoomType = roomType
            };

            if (checkIn.HasValue && checkOut.HasValue)
            {
                model.AvailableRooms = await _bookingService.GetAvailableRoomsAsync(checkIn.Value, checkOut.Value, guests);
                
                // Apply filters
                if (!string.IsNullOrEmpty(roomType))
                {
                    model.AvailableRooms = model.AvailableRooms.Where(r => r.Type.Contains(roomType, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Book(int roomId, DateTime checkIn, DateTime checkOut, int guests = 1)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return NotFound();

            // Check availability            if (!await _bookingService.IsRoomAvailableAsync(roomId, checkIn, checkOut))
            {
                TempData["Error"] = "This room is not available for the selected dates.";
                return RedirectToAction("Search");
            }var model = new BookingViewModel
            {
                RoomId = roomId,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NumberOfGuests = guests,
                Room = room,
                NumberOfNights = (checkOut - checkIn).Days,
                TotalNights = (checkOut - checkIn).Days,
                TotalPrice = await _bookingService.CalculateTotalPriceAsync(roomId, checkIn, checkOut)
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Book(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                // Double-check availability
                if (!await _bookingService.IsRoomAvailableAsync(model.RoomId, model.CheckInDate, model.CheckOutDate))
                {
                    ModelState.AddModelError("", "This room is no longer available for the selected dates.");
                    model.Room = await _context.Rooms.FindAsync(model.RoomId);
                    return View(model);
                }

                var booking = await _bookingService.CreateBookingAsync(model, userId);
                return RedirectToAction("Payment", new { bookingId = booking.Id });
            }

            model.Room = await _context.Rooms.FindAsync(model.RoomId);
            model.TotalPrice = await _bookingService.CalculateTotalPriceAsync(model.RoomId, model.CheckInDate, model.CheckOutDate);
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Payment(int bookingId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null) return NotFound();

            var model = new PaymentViewModel
            {
                BookingId = bookingId,
                Booking = booking,
                Amount = booking.TotalPrice
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Payment(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var payment = await _paymentService.ProcessPaymentAsync(model);
                    TempData["Success"] = "Payment successful! Your booking is confirmed.";
                    return RedirectToAction("Confirmation", new { bookingId = model.BookingId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Payment failed: " + ex.Message);
                }
            }

            // Reload booking data if validation fails
            model.Booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == model.BookingId);
            
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Confirmation(int bookingId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null) return NotFound();

            return View(booking);
        }

        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return View(bookings);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var booking = await _context.Bookings
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null) return NotFound();

            if (booking.Status == "Confirmed" && booking.CheckInDate > DateTime.Now.AddDays(1))
            {
                booking.Status = "Cancelled";
                
                // Process refund for completed payments
                foreach (var payment in booking.Payments.Where(p => p.Status == "Completed"))
                {
                    await _paymentService.RefundPaymentAsync(payment.Id);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking cancelled successfully. Refund will be processed.";
            }
            else
            {
                TempData["Error"] = "Cannot cancel this booking.";
            }

            return RedirectToAction("MyBookings");
        }
    }
}
