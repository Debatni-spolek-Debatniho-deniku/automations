using System.Text.Json;
using DSDD.Automations.Mailing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DSDD.Automations.SocialNetworks;

public class InstagramHttp
{
    public InstagramHttp(IMailingService mailingService)
    {
        _mailingService = mailingService;
    }

    [Function(nameof(InstagramHttp) + "-" + nameof(OnMessage))]
    public async Task<IActionResult> OnMessage([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
        [Microsoft.Azure.Functions.Worker.Http.FromBody] Body body, CancellationToken ct)
    {
        MailMessage message = new(
                [new MailParty(body.recipientName, body.recipientAddress)],
                body.subject,
                body.body);

        await _mailingService.SendAsync(message, ct);
        return new OkObjectResult("OK");
    }

    private readonly IMailingService _mailingService;

    public record Body(string recipientName, string recipientAddress, string subject, string body);
}
