using DSDD.Automations.RBCZ.PremiumApi.Sandbox;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Payments.RBCZ.PremiumApi;

internal class SandboxApiClient: IApiClient
{
    public SandboxApiClient(IOptions<RbczApiClientOptions> clientOptions)
    {
        clientId = clientOptions.Value.ClientId;
        auditIpAddress = clientOptions.Value.AuditIpAddress;

        X509Certificate2 certificate = new(
            clientOptions.Value.CertificatePath,
            clientOptions.Value.CertificatePassword);

        HttpClientHandler handler = new();
        handler.ClientCertificates.Add(certificate);

        _client = new(new(handler));
    }

    public async Task<decimal?> GetFxRateAsync(string nonCzkCurrency, CancellationToken ct)
    {
        CurrencyListSimple result = await _client.GetFxRatesAsync(
            clientId,
            RequestId(),
            auditIpAddress,
            nonCzkCurrency,
            null,
            ct);

        return result
            .ExchangeRateLists
            .OrderByDescending(l => l.OrdinalNumber)
            .Take(1)
            .SelectMany(l => l.ExchangeRates)
            .Select(r => (decimal?)r.ExchangeRateCenter)
            .FirstOrDefault();
    }

    private readonly Client _client;
    private readonly string clientId;
    private readonly string auditIpAddress;

    private string RequestId()
        => Guid.NewGuid().ToString();
}