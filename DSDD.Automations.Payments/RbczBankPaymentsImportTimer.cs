using DSDD.Automations.Payments.RBCZ;
using Microsoft.Azure.Functions.Worker;

namespace DSDD.Automations.Payments;

public class RbczBankPaymentsImportTimer
{
    public RbczBankPaymentsImportTimer(IBankPaymentsImporter importer)
    {
        _importer = importer;
    }

    [Function("RbczImportPaymentsTimer")]
    public Task Run([TimerTrigger("0 0 1 * * *"
#if DEBUG
        , RunOnStartup=true
#endif
    )] TimerInfo myTimer, CancellationToken ct)
        => _importer.ImportAsync(ct);

    private IBankPaymentsImporter _importer;
}
