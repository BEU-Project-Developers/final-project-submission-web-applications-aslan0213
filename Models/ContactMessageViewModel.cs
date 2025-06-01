using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class ContactMessageViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Your Name")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        [Display(Name = "Your Email")]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Phone Number (Optional)")]
        public string? PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters")]
        [Display(Name = "Your Message")]
        public string Message { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "Subject cannot exceed 100 characters")]
        [Display(Name = "Subject (Optional)")]
        public string? Subject { get; set; }
    }
}
