using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DSDD.Automations.DiscordMessages;

public class DiscordMessageOptions
{
    [ConfigurationKeyName("PRAHA_MESSAGE"), Required]
    public string PrahaMessage { get; set; } = "";

    [ConfigurationKeyName("PRAHA_WEBHOOK_URL"), Required]
    public string PrahaWebhookUrl { get; set; } = "";

    [ConfigurationKeyName("PLZEN_MESSAGE"), Required]
    public string PlzenMessage { get; set; } = "";

    [ConfigurationKeyName("PLZEN_WEBHOOK_URL"), Required]
    public string PlzenWebhookUrl { get; set; } = "";
}