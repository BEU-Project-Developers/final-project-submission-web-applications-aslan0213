using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using HotelManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers
{
    public class RoomsController : Controller
    {
        private readonly HotelDbContext _context;
        private readonly IBookingService _bookingService;

        public RoomsController(HotelDbContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index(string type = "", decimal? minPrice = null, decimal? maxPrice = null, int page = 1)
        {
            const int pageSize = 6;
            
            var query = _context.Rooms.Where(r => r.Status).AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(type))
                query = query.Where(r => r.Type.Contains(type));

            if (minPrice.HasValue)
                query = query.Where(r => r.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(r => r.Price <= maxPrice.Value);

            var totalRooms = await query.CountAsync();
            var rooms = await query
                .OrderBy(r => r.Price)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new SearchViewModel
            {
                AvailableRooms = rooms,
                RoomType = type,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                PageNumber = page,
                PageSize = pageSize,
                TotalResults = totalRooms,
                TotalPages = (int)Math.Ceiling(totalRooms / (double)pageSize)
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            // Get room reviews
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.RoomId == id && r.IsVerified)
                .OrderByDescending(r => r.ReviewDate)
                .Take(5)
                .ToListAsync();

            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            ViewBag.ReviewCount = await _context.Reviews.CountAsync(r => r.RoomId == id && r.IsVerified);

            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> Search(SearchViewModel model)
        {
            if (model.CheckInDate.HasValue && model.CheckOutDate.HasValue)
            {
                if (model.CheckInDate.Value <= DateTime.Today)
                {
                    ModelState.AddModelError("CheckInDate", "Check-in date must be in the future.");
                    return View("Index", model);
                }

                if (model.CheckOutDate.Value <= model.CheckInDate.Value)
                {
                    ModelState.AddModelError("CheckOutDate", "Check-out date must be after check-in date.");
                    return View("Index", model);
                }

                return RedirectToAction("Index", "Booking", new 
                { 
                    checkIn = model.CheckInDate.Value.ToString("yyyy-MM-dd"),
                    checkOut = model.CheckOutDate.Value.ToString("yyyy-MM-dd"),
                    guests = model.Guests,
                    roomType = model.RoomType
                });
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CheckAvailability(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var isAvailable = await _bookingService.IsRoomAvailableAsync(roomId, checkIn, checkOut);
            return Json(new { available = isAvailable });
        }
    }
}
