using DSDD.Automations.Payments.RBCZ;
using DSDD.Automations.Payments.Durable;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace DSDD.Automations.Payments;

public class RbczImport
{
    public RbczImport(IBankPaymentsImporter importer)
    {
        _importer = importer;
    }

    [Function(nameof(RbczImport) + "-" + nameof(Timer))]
    public Task Timer([TimerTrigger("%RBCZ_IMPORT_TIMER_CRON%")] TimerInfo myTimer, [DurableClient] DurableTaskClient client, CancellationToken ct)
        => client.ScheduleNewMethodOrchestrationInstanceAsync<RbczImport>(_ => _.Orhcestration(default!));

    [Function(nameof(RbczImport) + "-" + nameof(Orhcestration))]
    public Task Orhcestration([OrchestrationTrigger] TaskOrchestrationContext context)
        => context.CallActivityFromMethodTaskAsync<RbczImport>(_ => _.Activity(default!, default));

    [Function(nameof(RbczImport) + "-" + nameof(Activity))]
    public Task Activity(
        // Cannot be named _ (discard) as this name is prohibited in Azure Functions.
        [ActivityTrigger] TaskActivityContext context,
        CancellationToken ct)
        // TODO: rozbít importer na dílčí aktivity funkce.
        => _importer.ImportAsync(ct);

    private IBankPaymentsImporter _importer;
}
