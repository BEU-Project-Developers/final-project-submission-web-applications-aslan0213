using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers
{
    public class SearchController : Controller
    {
        private readonly HotelDbContext _context;

        public SearchController(HotelDbContext context)
        {
            _context = context;
        }        [HttpGet]
        public async Task<IActionResult> Index(string keyword = "", string roomType = "", decimal? minPrice = null, decimal? maxPrice = null, int guests = 1, int page = 1)
        {
            const int pageSize = 9;
            
            var query = _context.Rooms.Where(r => r.Status).AsQueryable();

            // Search by keyword (room number, type, or hotel name)
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(r => 
                    r.RoomNumber.Contains(keyword) ||
                    r.Type.Contains(keyword) ||
                    r.Description.Contains(keyword));
            }

            // Filter by room type
            if (!string.IsNullOrEmpty(roomType))
            {
                query = query.Where(r => r.Type == roomType);
            }

            // Filter by price range
            if (minPrice.HasValue)
                query = query.Where(r => r.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(r => r.Price <= maxPrice.Value);

            var totalResults = await query.CountAsync();
            var rooms = await query
                .OrderBy(r => r.Price)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();            var model = new SearchViewModel
            {
                Keyword = keyword,
                RoomType = roomType,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Guests = guests,
                NumberOfGuests = guests,
                AvailableRooms = rooms,
                Rooms = rooms,
                TotalResults = totalResults,
                PageNumber = page,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalResults / (double)pageSize)
            };

            // Get available room types for filter dropdown
            model.AvailableRoomTypes = await _context.Rooms
                .Where(r => r.Status)
                .Select(r => r.Type)
                .Distinct()
                .ToListAsync();

            ViewBag.RoomTypes = model.AvailableRoomTypes;

            return View(model);
        }

        [HttpPost]
        public IActionResult QuickSearch(string keyword)
        {
            return RedirectToAction("Index", new { keyword = keyword });
        }

        [HttpGet]
        public async Task<IActionResult> Suggestions(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
                return Json(new List<string>());

            var suggestions = await _context.Rooms
                .Where(r => r.Status && (
                    r.RoomNumber.Contains(term) ||
                    r.Type.Contains(term) ||
                    r.Description.Contains(term)))
                .Select(r => new { 
                    label = $"{r.Type} - Room {r.RoomNumber}",
                    value = r.Type,
                    roomId = r.Id
                })
                .Take(10)
                .ToListAsync();

            return Json(suggestions);
        }
    }
}
