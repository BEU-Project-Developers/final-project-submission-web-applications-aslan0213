using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class PaymentViewModel
    {
        public int BookingId { get; set; }
        
        [Required]
        [StringLength(19, MinimumLength = 13)]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; } = string.Empty;
          [Required]
        [StringLength(100)]
        [Display(Name = "Cardholder Name")]
        public string CardholderName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Cardholder Name")]
        public string CardHolderName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(5)]
        [Display(Name = "Expiry Date (MM/YY)")]
        public string ExpiryDate { get; set; } = string.Empty;
        
        [Required]
        [StringLength(4, MinimumLength = 3)]
        [Display(Name = "CVV")]
        public string CVV { get; set; } = string.Empty;
        
        [StringLength(200)]
        [Display(Name = "Billing Address")]
        public string BillingAddress { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Credit Card";
        
        // For display
        public Booking? Booking { get; set; }
        public decimal Amount { get; set; }
    }
}
