using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Role { get; set; } = "Guest";
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        public string? Address { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
