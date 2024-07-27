namespace DSDD.Automations.Payments.RBCZ;

public interface IBankPaymentsImporter
{
    Task ImportAsync(CancellationToken ct);
}