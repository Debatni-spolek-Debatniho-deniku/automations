namespace DSDD.Automations.Mailing;

public interface IMailingService
{
    Task SendAsync(MailMessage message, CancellationToken ct);
}