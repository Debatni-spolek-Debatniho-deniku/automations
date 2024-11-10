using Azure.Core;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace DSDD.Automations.Mailing;

public class MailKitMailingService: IMailingService, IDisposable
{
    public MailKitMailingService(IOptions<MailingOptions> options, TokenCredential tokenCredential)
    {
        _options = options;
        _tokenCredential = tokenCredential;
    }

    public async Task SendAsync(MailMessage message, CancellationToken ct)
    {
        MimeMessage mkMessage = new(
                [new MailboxAddress(_options.Value.SenderName, _options.Value.SenderEmail)],
                message.Recipients.Select(r => new MailboxAddress(r.Name, r.Address)),
                message.Subject,
                new TextPart(TextFormat.Html) { Text = message.Body });

        SmtpClient smtpClient = await GetSmtpClientAsync(ct);

        await smtpClient.SendAsync(mkMessage, ct);
    }

    public void Dispose()
    {
        if (_smtpClient is not null)
        {
            if (_smtpClient.IsConnected)
                _smtpClient.Disconnect(true);
            _smtpClient.Dispose();
        }
    }

    private SmtpClient? _smtpClient;
    private readonly IOptions<MailingOptions> _options;
    private readonly TokenCredential _tokenCredential;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private static string[] _scopes = new[] { "https://outlook.office365.com/.default" };

    private async Task<SmtpClient> GetSmtpClientAsync(CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);

        if (_smtpClient is null)
        {
            _smtpClient = new();
            _smtpClient.Connect(_options.Value.SmtpEndpoint, _options.Value.SmtpPort, SecureSocketOptions.StartTls);

            AccessToken accessToken = await _tokenCredential.GetTokenAsync(new(_scopes), ct);

            await _smtpClient.AuthenticateAsync(new SaslMechanismOAuth2(_options.Value.SenderEmail, accessToken.Token));
        }

        _semaphore.Release(1);

        return _smtpClient;
    }
}