using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public int RoomId { get; set; }
        
        [Required]
        public DateTime CheckInDate { get; set; }
        
        [Required]
        public DateTime CheckOutDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed
        
        public int NumberOfGuests { get; set; }
        
        public string? SpecialRequests { get; set; }
        
        public DateTime BookingDate { get; set; } = DateTime.Now;
        
        // Navigation properties
        public User User { get; set; } = null!;
        public Room Room { get; set; } = null!;
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
