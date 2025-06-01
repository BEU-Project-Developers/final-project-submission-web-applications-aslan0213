using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Address { get; set; } = string.Empty;
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        public string? Description { get; set; }
        
        public string? Image { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}
