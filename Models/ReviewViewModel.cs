using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{    public class ReviewViewModel
    {
        public int BookingId { get; set; }
        public int RoomId { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [StringLength(1000)]
        public string? Comment { get; set; }
        
        // For display
        public Booking? Booking { get; set; }
        public Room? Room { get; set; }
    }
}
