using HotelManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers
{
    public class StandardRoomController : Controller
    {
        private readonly HotelDbContext _context;

        public StandardRoomController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var standardRooms = await _context.Rooms
                .Where(r => r.Type == "Standard" && r.Status)
                .ToListAsync();

            return View(standardRooms);
        }

        public async Task<IActionResult> Details(int id)
        {
            var room = await _context.Rooms
                .Where(r => r.Id == id && r.Type == "Standard")
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
