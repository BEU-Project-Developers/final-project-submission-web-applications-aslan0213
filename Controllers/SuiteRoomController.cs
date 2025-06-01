using HotelManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers
{
    public class SuiteRoomController : Controller
    {
        private readonly HotelDbContext _context;

        public SuiteRoomController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var suiteRooms = await _context.Rooms
                .Where(r => r.Type == "Suite" && r.Status)
                .ToListAsync();

            return View(suiteRooms);
        }

        public async Task<IActionResult> Details(int id)
        {
            var room = await _context.Rooms
                .Where(r => r.Id == id && r.Type == "Suite")
                .FirstOrDefaultAsync();

            if (room == null) return NotFound();

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.RoomId == id && r.IsVerified)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            return View(room);
        }
    }
}
