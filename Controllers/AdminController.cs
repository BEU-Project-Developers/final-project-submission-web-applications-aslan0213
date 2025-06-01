using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly HotelDbContext _context;

        public AdminController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {            var model = new DashboardViewModel
            {
                TotalBookings = await _context.Bookings.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                TotalRooms = await _context.Rooms.CountAsync(),
                AvailableRooms = await _context.Rooms.CountAsync(r => r.IsAvailable),
                TotalRevenue = await _context.Payments.Where(p => p.Status == "Completed").SumAsync(p => p.Amount),
                
                TodayBookings = await _context.Bookings.CountAsync(b => b.BookingDate.Date == DateTime.Today),
                CheckInsToday = await _context.Bookings.CountAsync(b => b.CheckInDate.Date == DateTime.Today),
                CheckOutsToday = await _context.Bookings.CountAsync(b => b.CheckOutDate.Date == DateTime.Today),
                
                RecentBookings = await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Room)
                    .OrderByDescending(b => b.BookingDate)
                    .Take(5)
                    .ToListAsync(),
                    
                RecentPayments = await _context.Payments
                    .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(5)
                    .ToListAsync()
            };

            // Booking status statistics
            model.BookingsByStatus = await _context.Bookings
                .GroupBy(b => b.Status)
                .ToDictionaryAsync(g => g.Key, g => g.Count());            // Revenue by month (last 6 months)
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var paymentsData = await _context.Payments
                .Where(p => p.Status == "Completed" && p.PaymentDate >= sixMonthsAgo)
                .Select(p => new { p.PaymentDate, p.Amount })
                .ToListAsync();
              model.RevenueByMonth = paymentsData
                .GroupBy(p => p.PaymentDate.ToString("yyyy-MM"))
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

            return View("Dashboard", model);
        }

        // Room Management
        public async Task<IActionResult> Rooms()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return View(rooms);
        }

        public IActionResult CreateRoom()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(Room room)
        {
            if (ModelState.IsValid)
            {
                room.HotelId = 1; // Default hotel
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Room created successfully!";
                return RedirectToAction("Rooms");
            }
            return View(room);
        }

        public async Task<IActionResult> EditRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoom(Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Rooms.Update(room);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Room updated successfully!";
                return RedirectToAction("Rooms");
            }
            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                var hasBookings = await _context.Bookings.AnyAsync(b => b.RoomId == id);
                if (!hasBookings)
                {
                    _context.Rooms.Remove(room);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Room deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Cannot delete room with existing bookings.";
                }
            }
            return RedirectToAction("Rooms");
        }

        // Booking Management
        public async Task<IActionResult> Bookings(string status = "", DateTime? date = null)
        {
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(b => b.Status == status);

            if (date.HasValue)
                query = query.Where(b => b.CheckInDate.Date == date.Value.Date);

            var bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            ViewBag.SelectedStatus = status;
            ViewBag.SelectedDate = date;
            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(int id, string status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                booking.Status = status;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking status updated successfully!";
            }
            return RedirectToAction("Bookings");
        }

        // User Management
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(int id, string role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Role = role;
                await _context.SaveChangesAsync();
                TempData["Success"] = "User role updated successfully!";
            }
            return RedirectToAction("Users");
        }

        // Payment Management
        public async Task<IActionResult> Payments()
        {
            var payments = await _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.User)
                .Include(p => p.Booking)
                .ThenInclude(b => b.Room)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(payments);
        }

        // Reviews Management
        public async Task<IActionResult> Reviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Room)
                .Include(r => r.Booking)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Review deleted successfully!";
            }
            return RedirectToAction("Reviews");
        }
    }
}
