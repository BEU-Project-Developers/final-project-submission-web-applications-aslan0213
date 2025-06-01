using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class Review
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public int RoomId { get; set; }
        
        public int BookingId { get; set; }
        
        public string? Comment { get; set; }
        
        [Range(1, 5)]
        public int Rating { get; set; } // 1-5 stars
        
        public DateTime ReviewDate { get; set; } = DateTime.Now;
        
        public bool IsVerified { get; set; } = false; // Only after check-out
        
        // Navigation properties
        public User User { get; set; } = null!;
        public Room Room { get; set; } = null!;
        public Booking Booking { get; set; } = null!;
    }
}
