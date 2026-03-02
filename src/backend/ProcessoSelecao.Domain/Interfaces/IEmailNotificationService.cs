namespace ProcessoSelecao.Domain.Interfaces;

/// <summary>
/// Interface para serviço de envio de e-mails
/// </summary>
public interface IEmailNotificationService
{
    /// <summary>Envia uma notificação por e-mail</summary>
    /// <param name="to">Destinatário</param>
    /// <param name="subject">Assunto</param>
    /// <param name="body">Corpo do e-mail</param>
    Task SendNotificationAsync(string to, string subject, string body);
    
    /// <summary>Envia notificação para múltiplos destinatários</summary>
    /// <param name="recipients">Lista de destinatários</param>
    /// <param name="subject">Assunto</param>
    /// <param name="body">Corpo do e-mail</param>
    Task SendBulkNotificationAsync(IEnumerable<string> recipients, string subject, string body);
}
