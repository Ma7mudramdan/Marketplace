

using Marketplace.Application.DTOs.Email;
using Marketplace.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;

namespace Marketplace.Application.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(EmailDto emailDto)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("Email");
                var host = smtpSettings["SmtpServer"] ?? "smtp.gmail.com";
                var port = int.Parse(smtpSettings["SmtpPort"] ?? "587");
                var username = smtpSettings["Username"] ?? string.Empty;
                var password = smtpSettings["Password"] ?? string.Empty;
                var fromEmail = smtpSettings["FromEmail"] ?? username;

                using var client = new SmtpClient(host, port);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(username, password);

                using var message = new MailMessage();
                message.From  = new MailAddress(fromEmail);
                message.To.Add(emailDto.To);
                message.Subject = emailDto.Subject;
                message.Body = emailDto.Body;
                message.IsBodyHtml = emailDto.IsHtml;

                if(emailDto.Cc != null)
                {
                   foreach(var cc in emailDto.Cc) 
                        message.CC.Add(cc);
                }

                if(emailDto.Bcc != null)
                {
                    foreach(var bcc in emailDto.Bcc)
                        message.Bcc.Add(bcc);
                }

                if(emailDto.Attachments != null)
                {
                    foreach(var  attachment in emailDto.Attachments)
                    {
                        using var stream = new MemoryStream(attachment.Content);
                        var mailAttachment = new Attachment(stream,attachment.FileName,attachment.ContentType);

                        message.Attachments.Add(mailAttachment);
                    }
                }

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent to {To}", emailDto.To);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {To}", emailDto.To);
                return false;
            }
        }

        public async Task<bool> SendEmailVerificationAsync(string email, string verificationToken)
        {
            var subject = "Verify Your Email";
            var verifyLink = $"https://yourdomain.com/verify-email?token={verificationToken}&email={email}";

            var body = $@"
                <h1>Verify Your Email</h1>
                <p>Please click the link below to verify your email address:</p>
                <p><a href='{verifyLink}'>Verify Email</a></p>
                <p>This link will expire in 24 hours.</p>
                <br/>
                <p>Marketplace Team</p>
            ";

            var emailDto = new EmailDto
            {
                To = email,
                Subject = subject,
                Body = body,
                IsHtml = true,
            };

            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendOrderConfirmationAsync(string email, string name, int orderId)
        {
            var subject = $"Order #{orderId} Confirmation";
            var body = $@"
                <h1>Order Confirmation</h1>
                <p>Dear {name},</p>
                <p>Your order #{orderId} has been confirmed and is being processed.</p>
                <p>You will receive a notification when your order is shipped.</p>
                <br/>
                <p><a href='https://yourdomain.com/orders/{orderId}'>View Order Details</a></p>
                <br/>
                <p>Thank you for shopping with us!</p>
                <p>Marketplace Team</p>
            ";

            var emailDto = new EmailDto
            {
                Subject = subject,
                Body = body,
                IsHtml = true,
                To = email,
            };

            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendPasswordResetAsync(string email, string resetToken)
        {
            var subject = "Password Reset Request";
            var resetLink = $"https://yourdomain.com/reset-password?token={resetToken}&email={email}";

            var body = $@"
                <h1>Password Reset</h1>
                <p>We received a request to reset your password.</p>
                <p>Click the link below to reset your password:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you didn't request this, please ignore this email.</p>
                <br/>
                <p>Marketplace Team</p>
            ";

            var emailDto = new EmailDto
            {
                To = email,
                Subject = subject,
                Body = body,
                IsHtml= true,
            };

            return await SendEmailAsync(emailDto);

        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string name)
        {
            var subject = "Welcome to Marketplace!";
            var body = $@"
                <h1>Welcome {name}!</h1>
                <p>Thank you for joining Marketplace. We're excited to have you!</p>
                <p>You can now start buying and selling products.</p>
                <br/>
                <p>Best regards,<br/>Marketplace Team</p>
            ";

            var emailDto = new EmailDto
            {
                Subject = subject,
                Body = body,
                IsHtml = true,
                To = email,
            };

            return await SendEmailAsync(emailDto);
        }
    }
}
