using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementSystem.Models
{
    public class Payment
    {
        public int Id { get; set; }
        
        public int BookingId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Credit Card"; // Credit Card, Debit Card, PayPal, etc.
        
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded
        
        public string? TransactionId { get; set; }
        
        public string? PaymentNotes { get; set; }
        
        // Navigation properties
        public Booking Booking { get; set; } = null!;
    }
}
