namespace DSDD.Automations.Payments.Banking.RBCZ;

public interface IFxRates
{
    Task<decimal> ToCzkAsync(decimal value, string currencyCode, CancellationToken ct);
}