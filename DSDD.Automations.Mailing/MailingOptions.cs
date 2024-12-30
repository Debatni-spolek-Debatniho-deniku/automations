using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace DSDD.Automations.Mailing;

public class MailingOptions
{
    [ConfigurationKeyName("MAILING_SENDER_EMAIL"), Required]
    public string SenderEmail { get; set; } = "";

    [ConfigurationKeyName("MAILING_SENDER_NAME"), Required]
    public string SenderName { get; set; } = "";

    [ConfigurationKeyName("MAILING_SMTP_ENDPOINT"), Required]
    public string SmtpEndpoint { get; set; } = "";

    [ConfigurationKeyName("MAILING_SMTP_PORT"), Required]
    public int SmtpPort { get; set; }
}

