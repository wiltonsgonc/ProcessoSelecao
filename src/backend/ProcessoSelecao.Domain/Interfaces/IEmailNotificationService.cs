namespace ProcessoSelecao.Domain.Interfaces;

public interface IEmailNotificationService
{
    Task SendNotificationAsync(string to, string subject, string body);
    Task SendBulkNotificationAsync(IEnumerable<string> recipients, string subject, string body);
}
