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
    public Task Timer([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer, [DurableClient] DurableTaskClient client, CancellationToken ct)
        => client.ScheduleNewMethodOrchestrationInstanceAsync<RbczImport>(_ => _.Orhcestration(default!));

    [Function(nameof(RbczImport) + "-" + nameof(Orhcestration))]
    public Task Orhcestration([OrchestrationTrigger] TaskOrchestrationContext context)
        => context.CallActivityFromMethodTaskAsync<RbczImport>(_ => _.Activity(default!, default));

    [Function(nameof(RbczImport) + "-" + nameof(Activity))]
    public Task Activity([ActivityTrigger] TaskActivityContext _, CancellationToken ct)
        => _importer.ImportAsync(ct);

    private IBankPaymentsImporter _importer;
}
