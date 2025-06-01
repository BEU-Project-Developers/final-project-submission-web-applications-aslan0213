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
        }        public async Task<IActionResult> Index()
        {            
            var model = new DashboardViewModel
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
                .ToDictionaryAsync(g => g.Key, g => g.Count());            

            // Revenue by month (last 6 months)
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

        // Add Dashboard action method for direct access
        public async Task<IActionResult> Dashboard()
        {
            return await Index();
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
        }        [HttpPost]
        public async Task<IActionResult> CreateRoom(Room room, string amenitiesString)
        {
            if (ModelState.IsValid)
            {
                // Process amenities string
                if (!string.IsNullOrEmpty(amenitiesString))
                {
                    room.Amenities = amenitiesString.Split(',')
                        .Select(a => a.Trim())
                        .Where(a => !string.IsNullOrEmpty(a))
                        .ToList();
                }

                // Make sure Status matches IsAvailable (ensure consistency)
                room.Status = room.IsAvailable;
                
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
        }        [HttpPost]
        public async Task<IActionResult> EditRoom(Room room, string amenitiesString)
        {
            // Add debug information to TempData
            TempData["Debug"] = $"Received Room: ID={room.Id}, Number={room.RoomNumber}, Type={room.Type}, Price={room.Price}, Capacity={room.Capacity}, IsAvailable={room.IsAvailable}";
            
            if (ModelState.IsValid)
            {
                // Process amenities string
                if (!string.IsNullOrEmpty(amenitiesString))
                {
                    room.Amenities = amenitiesString.Split(',')
                        .Select(a => a.Trim())
                        .Where(a => !string.IsNullOrEmpty(a))
                        .ToList();
                }
                else
                {
                    room.Amenities = new List<string>();
                }
                
                // Make sure Status matches IsAvailable (ensure consistency)
                room.Status = room.IsAvailable;try {
                    _context.Rooms.Update(room);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Room updated successfully!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error updating room: {ex.Message}";
                    // Return to the rooms list even if there's an error
                    return RedirectToAction("Rooms");                }
                return RedirectToAction("Rooms");
            }
            
            // If we get here, there was a validation error
            TempData["Error"] = "There were validation errors. Please check your input.";
            var errorList = string.Join("; ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            TempData["Debug"] += $" | Validation errors: {errorList}";
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

        // Dashboard Quick Actions
        public IActionResult ManageBookings()
        {
            return RedirectToAction("Bookings");
        }

        public IActionResult ManageRooms()
        {
            return RedirectToAction("Rooms");
        }

        public IActionResult ManageUsers()
        {
            return RedirectToAction("Users");
        }

        // Booking Details
        public async Task<IActionResult> BookingDetails(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .Include(b => b.Payments)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("Bookings");
            }

            return View(booking);
        }

        // Reports
        public async Task<IActionResult> ViewReports()
        {
            var model = new ReportsViewModel
            {
                TotalBookings = await _context.Bookings.CountAsync(),
                TotalRevenue = await _context.Payments.Where(p => p.Status == "Completed").SumAsync(p => p.Amount),
                TotalUsers = await _context.Users.CountAsync(),
                TotalRooms = await _context.Rooms.CountAsync(),
                AvailableRooms = await _context.Rooms.CountAsync(r => r.IsAvailable),
                
                // Monthly revenue for the last 12 months
                MonthlyRevenue = await GetMonthlyRevenue(),
                
                // Booking status distribution
                BookingsByStatus = await _context.Bookings
                    .GroupBy(b => b.Status)
                    .ToDictionaryAsync(g => g.Key, g => g.Count()),
                
                // Room occupancy rate
                RoomOccupancyData = await GetRoomOccupancyData(),
                
                // Top customers
                TopCustomers = await _context.Bookings
                    .Include(b => b.User)
                    .GroupBy(b => b.UserId)
                    .Select(g => new CustomerReportData
                    {
                        CustomerName = g.First().User.FullName,
                        CustomerEmail = g.First().User.Email,
                        TotalBookings = g.Count(),
                        TotalSpent = g.Sum(b => b.TotalPrice)
                    })
                    .OrderByDescending(c => c.TotalSpent)
                    .Take(10)
                    .ToListAsync()
            };

            return View(model);
        }

        private async Task<Dictionary<string, decimal>> GetMonthlyRevenue()
        {
            var twelveMonthsAgo = DateTime.Now.AddMonths(-12);
            var paymentsData = await _context.Payments
                .Where(p => p.Status == "Completed" && p.PaymentDate >= twelveMonthsAgo)
                .Select(p => new { p.PaymentDate, p.Amount })
                .ToListAsync();

            return paymentsData
                .GroupBy(p => p.PaymentDate.ToString("yyyy-MM"))
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));
        }

        private async Task<Dictionary<string, object>> GetRoomOccupancyData()
        {
            var totalRooms = await _context.Rooms.CountAsync();
            var occupiedRooms = await _context.Bookings
                .Where(b => b.Status == "Confirmed" && 
                           b.CheckInDate <= DateTime.Now && 
                           b.CheckOutDate >= DateTime.Now)
                .CountAsync();

            var occupancyRate = totalRooms > 0 ? (double)occupiedRooms / totalRooms * 100 : 0;

            return new Dictionary<string, object>
            {
                { "TotalRooms", totalRooms },
                { "OccupiedRooms", occupiedRooms },
                { "AvailableRooms", totalRooms - occupiedRooms },
                { "OccupancyRate", Math.Round(occupancyRate, 1) }
            };
        }

        // Contact Messages Management
        public async Task<IActionResult> ContactMessages(int page = 1, bool? isRead = null)
        {
            var query = _context.ContactMessages.AsQueryable();
            
            if (isRead.HasValue)
            {
                query = query.Where(cm => cm.IsRead == isRead.Value);
            }
            
            var totalMessages = await query.CountAsync();
            var pageSize = 10;
            var totalPages = (int)Math.Ceiling(totalMessages / (double)pageSize);
            
            var messages = await query
                .OrderByDescending(cm => cm.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.IsReadFilter = isRead;
            ViewBag.TotalMessages = totalMessages;
            ViewBag.UnreadCount = await _context.ContactMessages.CountAsync(cm => !cm.IsRead);
            
            return View(messages);
        }

        [HttpGet]
        public async Task<IActionResult> ContactMessageDetails(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            
            // Mark as read if not already read
            if (!message.IsRead)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            
            return View(message);
        }

        [HttpPost]
        public async Task<IActionResult> AddAdminNotes(int id, string adminNotes)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            
            message.AdminNotes = adminNotes;
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Admin notes updated successfully.";
            return RedirectToAction("ContactMessageDetails", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
            {
                return Json(new { success = false, message = "Message not found" });
            }
            
            message.IsRead = true;
            message.ReadAt = DateTime.Now;
            await _context.SaveChangesAsync();
            
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsUnread(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
            {
                return Json(new { success = false, message = "Message not found" });
            }
            
            message.IsRead = false;
            message.ReadAt = null;
            await _context.SaveChangesAsync();
            
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteContactMessage(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
            {
                return Json(new { success = false, message = "Message not found" });
            }
            
            _context.ContactMessages.Remove(message);
            await _context.SaveChangesAsync();
            
            return Json(new { success = true });
        }
    }
}
