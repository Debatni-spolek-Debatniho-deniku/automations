namespace DSDD.Automations.Payments.Banking.Abstractions;

public interface IBankPaymentsImporter
{
    Task ImportAsync(CancellationToken ct);
}