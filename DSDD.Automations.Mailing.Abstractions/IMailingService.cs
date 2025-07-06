namespace DSDD.Automations.Mailing.Abstractions;

public interface IMailingService
{
    Task SendAsync(MailMessage message, CancellationToken ct);
}