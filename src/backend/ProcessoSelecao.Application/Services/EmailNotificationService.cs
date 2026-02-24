using System.Net.Mail;
using System.Net;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly EmailSettings _settings;

    public EmailNotificationService(EmailSettings settings)
    {
        _settings = settings;
    }

    public async Task SendNotificationAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPassword)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_settings.FromEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(to);

        await client.SendMailAsync(mailMessage);
    }

    public async Task SendBulkNotificationAsync(IEnumerable<string> recipients, string subject, string body)
    {
        var tasks = recipients.Select(r => SendNotificationAsync(r, subject, body));
        await Task.WhenAll(tasks);
    }
}

public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUser { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
}
