using DSDD.Automations.Hosting.Durable;
using DSDD.Automations.Hosting.Razor;
using DSDD.Automations.Mailing;
using DSDD.Automations.Reports.Reports;
using DSDD.Automations.Reports.Reports.MemberFees;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Reports;

public class NotifyMemberFees
{
    public NotifyMemberFees(IMailingService mailingService, IMemberFeesReport memberFeesReport,
        IOptions<NotifyMemberFeesOptions> notifyMemberFeesOptions)
    {
        _mailingService = mailingService;
        _memberFeesReport = memberFeesReport;
        _notifyMemberFeesOptions = notifyMemberFeesOptions;
    }

    [Function(nameof(NotifyMemberFees) + "-" + nameof(Timer))]
    public void Timer([TimerTrigger("%NOTIFY_TIMER_CRON%")] TimerInfo myTimer, [DurableClient] DurableTaskClient client)
        => client.ScheduleNewMethodOrchestrationInstanceAsync<NotifyMemberFees>(_ => _.Orhcestration(default!));


    [Function(nameof(NotifyMemberFees) + "-" + nameof(Orhcestration))]
    public async Task Orhcestration([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        await context.CallActivityFromMethodTaskAsync<NotifyMemberFees>(_ => _.SendMemberFeesReport(default!, default));
        await context.CallActivityFromMethodTaskAsync<NotifyMemberFees>(_ => _.SendMissingMemberFeesNotification(default!, default));
    }

    /// <summary>
    /// Send member fees report to the email address specified in the configuration.
    /// </summary>
    [Function(nameof(NotifyMemberFees) + "-" + nameof(SendMemberFeesReport) + "-" + "Activity")]
    public async Task SendMemberFeesReport([ActivityTrigger] TaskActivityContext context, CancellationToken ct)
    {
        ReportFile file = await _memberFeesReport.GenerateXlsxAsync(ct);

        MailParty recipient = new(
            _notifyMemberFeesOptions.Value.ReportRecipientName,
            _notifyMemberFeesOptions.Value.ReportRecipientEmail);

        await _mailingService.SendAsync(new(
            recipient,
            "Rozpis členských příspěvků",
            $"Tento e-mail obsahuje aktuální rozpis členských příspěvků ke dni {DateTime.Now:d}.",
            new MailAttachment(
                file.Name,
                file.ContentType,
                file.Content)), ct);
    }

    /// <summary>
    /// Sends warning email to every member that has not paid their monthly fee.
    /// </summary>
    [Function(nameof(NotifyMemberFees) + "-" + nameof(SendMissingMemberFeesNotification) + "-" + "Activity")]
    public Task SendMissingMemberFeesNotification([ActivityTrigger] TaskActivityContext context, CancellationToken ct)
        // TODO: Next stage
        => Task.CompletedTask;

    private readonly IMailingService _mailingService;
    private readonly IMemberFeesReport _memberFeesReport;
    private readonly IOptions<NotifyMemberFeesOptions> _notifyMemberFeesOptions;
}
