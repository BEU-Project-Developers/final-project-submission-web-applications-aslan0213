using MailKit.Net.Smtp;
using MimeKit;
using HotelManagementSystem.Models;

namespace HotelManagementSystem.Services
{
    public interface IEmailService
    {
        Task<bool> SendContactMessageEmailAsync(ContactMessage message);
        Task<bool> SendContactConfirmationEmailAsync(string toEmail, string name);
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string? plainTextContent = null);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendContactMessageEmailAsync(ContactMessage message)
        {
            try
            {
                var subject = $"New Contact Message from {message.Name}";
                if (!string.IsNullOrEmpty(message.Subject))
                {
                    subject = $"Contact: {message.Subject}";
                }

                var htmlContent = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <div style='background-color: #dfa974; color: white; padding: 20px; text-align: center;'>
                            <h2>New Contact Message - LuxeVoyage Hotel</h2>
                        </div>
                        <div style='padding: 20px; background-color: #f8f9fa;'>
                            <h3>Contact Details:</h3>
                            <table style='width: 100%; border-collapse: collapse;'>
                                <tr>
                                    <td style='padding: 8px; border: 1px solid #ddd; background-color: #f1f1f1; font-weight: bold;'>Name:</td>
                                    <td style='padding: 8px; border: 1px solid #ddd;'>{message.Name}</td>
                                </tr>
                                <tr>
                                    <td style='padding: 8px; border: 1px solid #ddd; background-color: #f1f1f1; font-weight: bold;'>Email:</td>
                                    <td style='padding: 8px; border: 1px solid #ddd;'>{message.Email}</td>
                                </tr>";

                if (!string.IsNullOrEmpty(message.PhoneNumber))
                {
                    htmlContent += $@"
                                <tr>
                                    <td style='padding: 8px; border: 1px solid #ddd; background-color: #f1f1f1; font-weight: bold;'>Phone:</td>
                                    <td style='padding: 8px; border: 1px solid #ddd;'>{message.PhoneNumber}</td>
                                </tr>";
                }

                if (!string.IsNullOrEmpty(message.Subject))
                {
                    htmlContent += $@"
                                <tr>
                                    <td style='padding: 8px; border: 1px solid #ddd; background-color: #f1f1f1; font-weight: bold;'>Subject:</td>
                                    <td style='padding: 8px; border: 1px solid #ddd;'>{message.Subject}</td>
                                </tr>";
                }

                htmlContent += $@"
                                <tr>
                                    <td style='padding: 8px; border: 1px solid #ddd; background-color: #f1f1f1; font-weight: bold;'>Date:</td>
                                    <td style='padding: 8px; border: 1px solid #ddd;'>{message.CreatedAt:dddd, MMMM dd, yyyy 'at' HH:mm}</td>
                                </tr>
                            </table>
                            
                            <h3 style='margin-top: 20px;'>Message:</h3>
                            <div style='background-color: white; padding: 15px; border: 1px solid #ddd; border-radius: 5px;'>
                                {message.Message.Replace("\n", "<br>")}
                            </div>
                            
                            <div style='margin-top: 20px; padding: 10px; background-color: #e9ecef; border-left: 4px solid #dfa974;'>
                                <small><strong>Note:</strong> Please respond to this message using the customer's email address: {message.Email}</small>
                            </div>
                        </div>
                    </div>";

                var plainTextContent = $@"
New Contact Message - LuxeVoyage Hotel

Contact Details:
Name: {message.Name}
Email: {message.Email}
{(!string.IsNullOrEmpty(message.PhoneNumber) ? $"Phone: {message.PhoneNumber}" : "")}
{(!string.IsNullOrEmpty(message.Subject) ? $"Subject: {message.Subject}" : "")}
Date: {message.CreatedAt:dddd, MMMM dd, yyyy 'at' HH:mm}

Message:
{message.Message}

Note: Please respond to this message using the customer's email address: {message.Email}";                
                var hotelEmail = _configuration["EmailSettings:HotelEmail"] ?? "aslanmamedov0213@gmail.com";
                return await SendEmailAsync(hotelEmail, subject, htmlContent, plainTextContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send contact message email");
                return false;
            }
        }

        public async Task<bool> SendContactConfirmationEmailAsync(string toEmail, string name)
        {
            try
            {
                var subject = "Thank you for contacting LuxeVoyage Hotel";

                var htmlContent = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <div style='background-color: #dfa974; color: white; padding: 20px; text-align: center;'>
                            <h2>Thank You for Contacting Us!</h2>
                        </div>
                        <div style='padding: 20px; background-color: #f8f9fa;'>
                            <p>Dear {name},</p>
                            
                            <p>Thank you for reaching out to LuxeVoyage Hotel. We have received your message and appreciate your interest in our services.</p>
                            
                            <p>Our team will review your inquiry and respond within 24 hours. If your matter is urgent, please don't hesitate to call us directly at <strong>+994 50 598 69 33</strong>.</p>
                              <div style='background-color: white; padding: 15px; border: 1px solid #ddd; border-radius: 5px; margin: 20px 0;'>
                                <h4 style='color: #dfa974; margin-top: 0;'>Contact Information:</h4>
                                <p style='margin: 5px 0;'><strong>Phone:</strong> +994 50 598 69 33</p>
                                <p style='margin: 5px 0;'><strong>Email:</strong> aslanmamedov0213@gmail.com</p>
                                <p style='margin: 5px 0;'><strong>Address:</strong> FR99+HPM, Jafar Jabbarli, Baku, Azerbaijan</p>
                            </div>
                            
                            <p>We look forward to welcoming you to LuxeVoyage Hotel and providing you with an exceptional hospitality experience.</p>
                            
                            <p>Best regards,<br>
                            <strong>LuxeVoyage Hotel Team</strong></p>
                        </div>
                        <div style='background-color: #333; color: white; padding: 15px; text-align: center; font-size: 12px;'>
                            <p>This is an automated message. Please do not reply to this email.</p>
                        </div>
                    </div>";

                var plainTextContent = $@"
Thank You for Contacting Us!

Dear {name},

Thank you for reaching out to LuxeVoyage Hotel. We have received your message and appreciate your interest in our services.

Our team will review your inquiry and respond within 24 hours. If your matter is urgent, please don't hesitate to call us directly at +994 50 598 69 33.

Contact Information:
Phone: +994 50 598 69 33
Email: aslanmamedov0213@gmail.com
Address: FR99+HPM, Jafar Jabbarli, Baku, Azerbaijan

We look forward to welcoming you to LuxeVoyage Hotel and providing you with an exceptional hospitality experience.

Best regards,
LuxeVoyage Hotel Team

This is an automated message. Please do not reply to this email.";

                return await SendEmailAsync(toEmail, subject, htmlContent, plainTextContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send contact confirmation email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string? plainTextContent = null)
        {
            try
            {
                var message = new MimeMessage();
                  // From
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "aslanmamedov0213@gmail.com";
                var fromName = _configuration["EmailSettings:FromName"] ?? "LuxeVoyage Hotel";
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                
                // To
                message.To.Add(new MailboxAddress("", toEmail));
                
                // Subject
                message.Subject = subject;
                
                // Body
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = htmlContent;
                
                if (!string.IsNullOrEmpty(plainTextContent))
                {
                    bodyBuilder.TextBody = plainTextContent;
                }
                
                message.Body = bodyBuilder.ToMessageBody();

                // Send using SMTP
                using var client = new SmtpClient();
                
                // For development, we'll use a simple SMTP configuration
                // In production, you should use proper SMTP settings
                var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? fromEmail;
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");                // Check if real email sending is enabled (when SMTP password is provided)
                var enableRealEmails = !string.IsNullOrEmpty(smtpPassword);
                
                if (!enableRealEmails)
                {
                    // Log the email instead of sending it when no SMTP password is configured
                    _logger.LogInformation("[DEVELOPMENT MODE] Email that would be sent to {ToEmail}:", toEmail);
                    _logger.LogInformation("Subject: {Subject}", subject);
                    _logger.LogInformation("HTML Content: {HtmlContent}", htmlContent);
                    _logger.LogInformation("--- End of Email Content ---");
                    return true;
                }

                // Validate email configuration
                _logger.LogInformation("Email Configuration Check:");
                _logger.LogInformation("SMTP Host: {SmtpHost}", smtpHost);
                _logger.LogInformation("SMTP Port: {SmtpPort}", smtpPort);
                _logger.LogInformation("SMTP Username: {SmtpUsername}", smtpUsername);
                _logger.LogInformation("Enable SSL: {EnableSsl}", enableSsl);
                _logger.LogInformation("Password Length: {PasswordLength}", smtpPassword?.Length ?? 0);
                _logger.LogInformation("From Email: {FromEmail}", fromEmail);
                _logger.LogInformation("To Email: {ToEmail}", toEmail);// Real email sending when SMTP credentials are configured
                _logger.LogInformation("Attempting to send email to {ToEmail} using SMTP Host: {SmtpHost}:{SmtpPort}, SSL: {EnableSsl}, Username: {SmtpUsername}", 
                    toEmail, smtpHost, smtpPort, enableSsl, smtpUsername);

                // Try multiple Gmail SMTP configurations
                var smtpConfigurations = new[]
                {
                    new { Host = smtpHost, Port = smtpPort, UseSsl = enableSsl, Description = "Primary config (587 TLS)" },
                    new { Host = smtpHost, Port = 465, UseSsl = true, Description = "Alternative config (465 SSL)" },
                    new { Host = smtpHost, Port = 587, UseSsl = true, Description = "Fallback config (587 SSL)" }
                };

                Exception lastException = null;

                foreach (var config in smtpConfigurations)
                {
                    try
                    {
                        _logger.LogInformation("Trying {Description}: {Host}:{Port}, SSL: {UseSsl}", 
                            config.Description, config.Host, config.Port, config.UseSsl);

                        using var testClient = new SmtpClient();
                        
                        _logger.LogInformation("Connecting to SMTP server...");
                        await testClient.ConnectAsync(config.Host, config.Port, config.UseSsl);
                        _logger.LogInformation("Connected successfully to SMTP server");

                        _logger.LogInformation("Authenticating with SMTP server...");
                        await testClient.AuthenticateAsync(smtpUsername, smtpPassword);
                        _logger.LogInformation("Authentication successful");

                        _logger.LogInformation("Sending email...");
                        await testClient.SendAsync(message);
                        _logger.LogInformation("Email sent successfully");

                        await testClient.DisconnectAsync(true);
                        _logger.LogInformation("Disconnected from SMTP server");

                        _logger.LogInformation("Email sent successfully to {ToEmail} using {Description}", toEmail, config.Description);
                        return true;
                    }
                    catch (MailKit.Security.AuthenticationException authEx)
                    {
                        _logger.LogWarning("SMTP Authentication failed for {Description}. Username: {Username}, Error: {Error}", 
                            config.Description, smtpUsername, authEx.Message);
                        lastException = authEx;
                        continue;
                    }
                    catch (MailKit.Net.Smtp.SmtpCommandException smtpEx)
                    {
                        _logger.LogWarning("SMTP Command failed for {Description}. Status Code: {StatusCode}, Message: {Message}", 
                            config.Description, smtpEx.StatusCode, smtpEx.Message);
                        lastException = smtpEx;
                        continue;
                    }
                    catch (Exception connectEx)
                    {
                        _logger.LogWarning("Failed to connect for {Description}. Error: {Error}", 
                            config.Description, connectEx.Message);
                        lastException = connectEx;
                        continue;
                    }
                }

                // If all configurations failed, log the last exception
                if (lastException != null)
                {
                    _logger.LogError(lastException, "All SMTP configurations failed for {ToEmail}", toEmail);
                    throw lastException;
                }

                throw new InvalidOperationException("No SMTP configurations were attempted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}. Error Type: {ErrorType}", toEmail, ex.GetType().Name);
                return false;
            }
        }
    }
}
