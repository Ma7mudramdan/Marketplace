
using Marketplace.Application.DTOs.Email;

namespace Marketplace.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailDto emailDto);
        Task<bool> SendWelcomeEmailAsync(string email, string name);
        Task<bool> SendOrderConfirmationAsync(string email, string name, int orderId);
        Task<bool> SendPasswordResetAsync(string email, string resetToken);
        Task<bool> SendEmailVerificationAsync(string email, string verificationToken);
    }
}