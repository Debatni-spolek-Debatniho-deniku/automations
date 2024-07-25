namespace DSDD.Automations.Payments.RBCZ.PremiumApi;

internal interface IPremiumApiClient
{
    /// <summary>
    /// Returns FX rate from non-CZK currency to CZK currency.
    /// </summary>
    /// <param name="nonCzkCurrency"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<ExchangeRate?> GetFxRateAsync(string nonCzkCurrency, CancellationToken ct);

    /// <summary>
    /// Returs all transactions that occured in last 90 days.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    IAsyncEnumerable<Transaction> GetLast90DaysTransactionsAsync(CancellationToken ct);
}