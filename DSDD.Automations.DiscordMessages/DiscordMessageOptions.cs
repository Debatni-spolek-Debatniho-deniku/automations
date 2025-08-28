using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DSDD.Automations.DiscordMessages;

public class DiscordMessageOptions
{
    [ConfigurationKeyName("TIMER_1_MESSAGE"), Required]
    public string Timer1Message { get; set; } = "";

    [ConfigurationKeyName("TIMER_1_WEBHOOK_URL"), Required]
    public string Timer1WebhookUrl { get; set; } = "";

    [ConfigurationKeyName("TIMER_2_MESSAGE"), Required]
    public string Timer2Message { get; set; } = "";

    [ConfigurationKeyName("TIMER_2_WEBHOOK_URL"), Required]
    public string Timer2WebhookUrl { get; set; } = "";

    [ConfigurationKeyName("TIMER_3_MESSAGE"), Required]
    public string Timer3Message { get; set; } = "";

    [ConfigurationKeyName("TIMER_3_WEBHOOK_URL"), Required]
    public string Timer3WebhookUrl { get; set; } = "";

    [ConfigurationKeyName("TIMER_4_MESSAGE"), Required]
    public string Timer4Message { get; set; } = "";

    [ConfigurationKeyName("TIMER_4_WEBHOOK_URL"), Required]
    public string Timer4WebhookUrl { get; set; } = "";
}