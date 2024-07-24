namespace DSDD.Automations.Payments.RBCZ.PremiumApi;

internal class ProductionPremiumApiClient: IApiClient
{
    public Task<decimal?> GetFxRateAsync(string nonCzkCurrency, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}