namespace HotelManagementSystem.Models
{
    public class ReportsViewModel
    {
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalUsers { get; set; }
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        
        public Dictionary<string, decimal> MonthlyRevenue { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> BookingsByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, object> RoomOccupancyData { get; set; } = new Dictionary<string, object>();
        public List<CustomerReportData> TopCustomers { get; set; } = new List<CustomerReportData>();
    }
    
    public class CustomerReportData
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
