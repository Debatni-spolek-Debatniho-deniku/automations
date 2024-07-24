using System.Collections.Concurrent;
using DSDD.Automations.Payments.RBCZ.PremiumApi;

namespace DSDD.Automations.Payments.RBCZ;

internal class FxRates: IFxRates
{
    public FxRates(IApiClient client)
    {
        _client = client;
    }

    public async Task<decimal> ToCzkAsync(decimal value, string currencyCode, CancellationToken ct)
    {
        SemaphoreSlim currencySemaphore = _currencySmaphores.GetOrAdd(currencyCode, _ => new SemaphoreSlim(1, 1));

        try
        {
            await currencySemaphore.WaitAsync(ct);

            if (!_fxRateCache.TryGetValue(currencyCode, out decimal toCzkRate))
            {
                decimal? maybeRate = await _client.GetFxRateAsync(currencyCode, ct);
                if (maybeRate is null)
                    throw new InvalidOperationException($"No currency rate is known from {currencyCode} to CZK!");
                toCzkRate = maybeRate.Value;

                _fxRateCache.Add(currencyCode, toCzkRate);
            }

            return value * toCzkRate;
        }
        finally
        {
            currencySemaphore.Release();
        }
    }

    private readonly IApiClient _client;

    private readonly ConcurrentDictionary<string, SemaphoreSlim> _currencySmaphores = new();
    private readonly Dictionary<string, decimal> _fxRateCache = new();
}