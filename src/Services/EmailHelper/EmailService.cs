using Konoha.Common;
using Konoha.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Konoha.Services.EmailHelper;

public class EmailService(
    ISmtpClient smtpClient,
    ILogger<EmailService> logger,
    IConfiguration configuration
) : IEmailService
{
    private readonly ISmtpClient _smtpClient = smtpClient;
    private readonly ILogger<EmailService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    public async Task SendEmailAsync(EmailModel email)
    {
        try
        {
            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress("Konoha", "rajatchaduhary6399@gmail.com"));
            foreach (var reciever in email.ToEmails)
            {
                mail.To.Add(MailboxAddress.Parse(reciever));
            }
            mail.Subject = email.Subject;
            mail.Body = new TextPart("plain") { Text = email.Body };
            await _smtpClient.SendAsync(mail);
            _logger.LogInformation("EMAIL SENT");
        }
        catch (Exception ex)
        {
            throw new KonohaException(
                KonohaException.InternalServerError,
                $"Failed to Send Email: {ex.Message}"
            );
        }
    }
}
