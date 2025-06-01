using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using HotelManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelManagementSystem.Controllers
{
    public class ReviewController : Controller
    {
        private readonly HotelDbContext _context;
        private readonly IBookingService _bookingService;

        public ReviewController(HotelDbContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        [Authorize]
        public async Task<IActionResult> Create(int bookingId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (!await _bookingService.CanUserReviewBookingAsync(bookingId, userId))
            {
                TempData["Error"] = "You cannot review this booking.";
                return RedirectToAction("MyBookings", "Booking");
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null) return NotFound();            var model = new ReviewViewModel
            {
                BookingId = bookingId,
                RoomId = booking.RoomId,
                Booking = booking,
                Room = booking.Room
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (!await _bookingService.CanUserReviewBookingAsync(model.BookingId, userId))
                {
                    TempData["Error"] = "You cannot review this booking.";
                    return RedirectToAction("MyBookings", "Booking");
                }

                var booking = await _context.Bookings.FindAsync(model.BookingId);
                if (booking == null) return NotFound();

                var review = new Review
                {
                    UserId = userId,
                    RoomId = booking.RoomId,
                    BookingId = model.BookingId,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    ReviewDate = DateTime.Now,
                    IsVerified = true
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thank you for your review!";
                return RedirectToAction("Details", "Booking", new { id = model.BookingId });
            }

            // Reload data if validation fails
            model.Booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == model.BookingId);

            return View(model);
        }

        public async Task<IActionResult> RoomReviews(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return NotFound();

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.RoomId == roomId && r.IsVerified)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            ViewBag.Room = room;
            ViewBag.AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            
            return View(reviews);
        }

        [Authorize]
        public async Task<IActionResult> MyReviews()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var reviews = await _context.Reviews
                .Include(r => r.Room)
                .Include(r => r.Booking)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            return View(reviews);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Review deleted successfully!";
            }

            return RedirectToAction("MyReviews");
        }
    }
}
