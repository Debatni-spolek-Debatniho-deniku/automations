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

    [Function(nameof(DiscordTimer) + "-" + nameof(Timer1))]
    public Task Timer1([TimerTrigger("%TIMER_1_CRON%")] TimerInfo myTimer)
    {
        _logger.LogInformation("Notifying: Timer 1");

        return notify(_discordOptions.Value.Timer1WebhookUrl, _discordOptions.Value.Timer1Message);
    }

    [Function(nameof(DiscordTimer) + "-" + nameof(Timer2))]
    public Task Timer2([TimerTrigger("%TIMER_2_CRON%")] TimerInfo myTimer)
    {
        _logger.LogInformation("Notifying: Timer 2");

        return notify(_discordOptions.Value.Timer2WebhookUrl, _discordOptions.Value.Timer2Message);
    }

    [Function(nameof(DiscordTimer) + "-" + nameof(Timer3))]
    public Task Timer3([TimerTrigger("%TIMER_3_CRON%")] TimerInfo myTimer)
    {
        _logger.LogInformation("Notifying: Timer 3");

        return notify(_discordOptions.Value.Timer3WebhookUrl, _discordOptions.Value.Timer3Message);
    }

    [Function(nameof(DiscordTimer) + "-" + nameof(Timer4))]
    public Task Timer4([TimerTrigger("%TIMER_4_CRON%")] TimerInfo myTimer)
    {
        _logger.LogInformation("Notifying: Timer 4");

        return notify(_discordOptions.Value.Timer4WebhookUrl, _discordOptions.Value.Timer4Message);
    }

    private readonly ILogger _logger;
    private readonly IOptions<DiscordMessageOptions> _discordOptions;

    private async Task notify(string webhookUrl, string message)
    {
        using DiscordWebhookClient client = new DiscordWebhookClient(webhookUrl);

        await client.SendMessageAsync(message);
    }
}
