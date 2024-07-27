namespace DSDD.Automations.Payments.RBCZ;

public interface IFxRates
{
    Task<decimal> ToCzkAsync(decimal value, string currencyCode, CancellationToken ct);
}