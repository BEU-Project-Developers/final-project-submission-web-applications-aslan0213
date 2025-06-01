using Microsoft.AspNetCore.Mvc;
using HotelManagementSystem.Models;
using HotelManagementSystem.Services;
using HotelManagementSystem.Data;

namespace HotelManagementSystem.Controllers
{
    public class ContactController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly HotelDbContext _context;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IEmailService emailService, HotelDbContext context, ILogger<ContactController> logger)
        {
            _emailService = emailService;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactMessageViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactMessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Create contact message entity
                var contactMessage = new ContactMessage
                {
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Message = model.Message,
                    Subject = model.Subject,
                    CreatedAt = DateTime.Now
                };

                // Save to database
                _context.ContactMessages.Add(contactMessage);
                await _context.SaveChangesAsync();

                // Send email to admin
                var emailSent = await _emailService.SendContactMessageEmailAsync(contactMessage);
                
                if (emailSent)
                {
                    // Send confirmation email to customer
                    await _emailService.SendContactConfirmationEmailAsync(model.Email, model.Name);
                    
                    TempData["SuccessMessage"] = "Thank you for your message! We have received your inquiry and will get back to you soon.";
                    _logger.LogInformation("Contact message sent successfully from {Email} at {Time}", model.Email, DateTime.Now);
                }
                else
                {
                    TempData["WarningMessage"] = "Your message has been saved but there was an issue sending the email notification. We will still get back to you soon.";
                    _logger.LogWarning("Contact message saved but email sending failed for {Email} at {Time}", model.Email, DateTime.Now);
                }

                // Clear the form after successful submission
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact message from {Email} at {Time}", model.Email, DateTime.Now);
                TempData["ErrorMessage"] = "There was an error processing your message. Please try again later.";
                return View(model);
            }
        }
    }
}
