namespace HotelManagementSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalBookings { get; set; }
        public int TotalUsers { get; set; }        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public decimal TotalRevenue { get; set; }
        
        public int TodayBookings { get; set; }
        public int CheckInsToday { get; set; }
        public int CheckOutsToday { get; set; }
        
        public List<Booking> RecentBookings { get; set; } = new List<Booking>();
        public List<Payment> RecentPayments { get; set; } = new List<Payment>();
        public Dictionary<string, int> BookingsByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> RevenueByMonth { get; set; } = new Dictionary<string, decimal>();
    }
}
