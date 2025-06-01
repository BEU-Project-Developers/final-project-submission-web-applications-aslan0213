namespace HotelManagementSystem.Models
{
    public class SearchViewModel
    {
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int Guests { get; set; } = 1;
        public int NumberOfGuests { get; set; } = 1;
        public string? RoomType { get; set; }
        public string? Keyword { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        
        // Results
        public List<Room> AvailableRooms { get; set; } = new List<Room>();
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<string> AvailableRoomTypes { get; set; } = new List<string>();
        public int TotalResults { get; set; }
        public int PageNumber { get; set; } = 1;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public int TotalPages { get; set; }
    }
}
