namespace DSDD.Automations.Payments.RBCZ.PremiumApi;

internal interface IApiClient
{
    Task<decimal?> GetFxRateAsync(string nonCzkCurrency, CancellationToken ct);
}