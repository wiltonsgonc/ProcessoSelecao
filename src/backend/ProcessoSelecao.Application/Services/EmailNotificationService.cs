using System.Net.Mail;
using System.Net;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Application.Services;

/// <summary>
/// Serviço para envio de notificações por e-mail
/// </summary>
public class EmailNotificationService : IEmailNotificationService
{
    private readonly EmailSettings _settings;

    public EmailNotificationService(EmailSettings settings)
    {
        _settings = settings;
    }

    /// <summary>Envia uma notificação por e-mail</summary>
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

    /// <summary>Envia notificação para múltiplos destinatários</summary>
    public async Task SendBulkNotificationAsync(IEnumerable<string> recipients, string subject, string body)
    {
        var tasks = recipients.Select(r => SendNotificationAsync(r, subject, body));
        await Task.WhenAll(tasks);
    }
}

/// <summary>
/// Configurações para envio de e-mails
/// </summary>
public class EmailSettings
{
    /// <summary>Servidor SMTP</summary>
    public string SmtpHost { get; set; } = string.Empty;
    
    /// <summary>Porta SMTP</summary>
    public int SmtpPort { get; set; } = 587;
    
    /// <summary>Usuário SMTP</summary>
    public string SmtpUser { get; set; } = string.Empty;
    
    /// <summary>Senha SMTP</summary>
    public string SmtpPassword { get; set; } = string.Empty;
    
    /// <summary>E-mail de origem</summary>
    public string FromEmail { get; set; } = string.Empty;
}
