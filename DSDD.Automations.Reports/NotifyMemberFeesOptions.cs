using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DSDD.Automations.Reports;

public class NotifyMemberFeesOptions
{
    [ConfigurationKeyName("NOTIFY_MEMBER_FEES_REPORT_RECIPIENT_EMAIL"), Required]
    public string ReportRecipientEmail { get; set; } = "";

    [ConfigurationKeyName("NOTIFY_MEMBER_FEES_REPORT_RECIPIENT_NAME"), Required]
    public string ReportRecipientName { get; set; } = "";
}