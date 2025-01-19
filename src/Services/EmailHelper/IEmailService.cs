using Konoha.Models;
using MimeKit;

namespace Konoha.Services.EmailHelper
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailModel email);
    }
}