using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Services
{
    public interface IPaymentService
    {
        Task<Payment> ProcessPaymentAsync(PaymentViewModel model);
        Task<bool> RefundPaymentAsync(int paymentId);
        Task<List<Payment>> GetBookingPaymentsAsync(int bookingId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly HotelDbContext _context;

        public PaymentService(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> ProcessPaymentAsync(PaymentViewModel model)
        {
            
            var booking = await _context.Bookings.FindAsync(model.BookingId);
            if (booking == null)
                throw new ArgumentException("Booking not found");

            var payment = new Payment
            {
                BookingId = model.BookingId,
                Amount = booking.TotalPrice,
                PaymentDate = DateTime.Now,
                PaymentMethod = model.PaymentMethod,
                Status = "Completed", 
                TransactionId = GenerateTransactionId()
            };

            _context.Payments.Add(payment);
            
            booking.Status = "Confirmed";
            
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> RefundPaymentAsync(int paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null || payment.Status != "Completed")
                return false;

            payment.Status = "Refunded";
            payment.Booking.Status = "Cancelled";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Payment>> GetBookingPaymentsAsync(int bookingId)
        {
            return await _context.Payments
                .Where(p => p.BookingId == bookingId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        private string GenerateTransactionId()
        {
            return "TXN" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999);
        }
    }
}
