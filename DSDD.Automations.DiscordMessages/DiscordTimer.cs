using Discord.Webhook;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.DiscordMessages;

public class DiscordTimer
{
    public DiscordTimer(ILoggerFactory loggerFactory, IOptions<DiscordMessageOptions> discordOptions)
    {
        _logger = loggerFactory.CreateLogger<DiscordTimer>();
        _discordOptions = discordOptions;
    }

    [Function(nameof(DiscordTimer) + "-" + nameof(Praha))]
    public Task Praha([TimerTrigger("%PRAHA_TIMER_CRON%")] TimerInfo myTimer)
    {
        _logger.LogInformation("Notifying: Praha");

        return notify(_discordOptions.Value.PrahaWebhookUrl, _discordOptions.Value.PrahaMessage);
    }

    [Function(nameof(DiscordTimer) + "-" + nameof(Plzen))]
    public Task Plzen([TimerTrigger("%PLZEN_TIMER_CRON%")] TimerInfo myTimer)
    {
        _logger.LogInformation("Notifying: Plzen");

        return notify(_discordOptions.Value.PlzenWebhookUrl, _discordOptions.Value.PlzenMessage);
    }

    private readonly ILogger _logger;
    private readonly IOptions<DiscordMessageOptions> _discordOptions;

    private async Task notify(string webhookUrl, string message)
    {
        using DiscordWebhookClient client = new DiscordWebhookClient(webhookUrl);

        await client.SendMessageAsync(message);
    }
}
