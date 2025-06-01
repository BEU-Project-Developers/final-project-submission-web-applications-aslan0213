using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class BookingViewModel
    {
        public int RoomId { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }
        
        [Required]
        [Range(1, 10)]
        public int NumberOfGuests { get; set; }
        
        public string? SpecialRequests { get; set; }
          // For display
        public Room? Room { get; set; }
        public decimal TotalPrice { get; set; }
        public int NumberOfNights { get; set; }
        public int TotalNights { get; set; }
    }
}
