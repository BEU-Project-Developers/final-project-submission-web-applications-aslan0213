using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementSystem.Models
{
    public class Room
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(10)]
        public string RoomNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        public bool Status { get; set; } = true; // true = available, false = unavailable
        public bool IsAvailable { get; set; } = true;
        
        public int HotelId { get; set; }
        
        public string? Image { get; set; }
        
        public int Capacity { get; set; }
        
        public string? Description { get; set; }
        
        public List<string> Amenities { get; set; } = new List<string>();
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
