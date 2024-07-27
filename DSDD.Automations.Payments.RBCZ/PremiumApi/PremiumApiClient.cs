using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Payments.RBCZ.PremiumApi;

internal class PremiumApiClient: IPremiumApiClient
{
    public PremiumApiClient(IOptions<PremiumApiClientOptions> clientOptions, ILogger<PremiumApiClient> logger)
    {
        _clientId = clientOptions.Value.ClientId;
        _auditIpAddress = clientOptions.Value.AuditIpAddress;
        _accountNumber = clientOptions.Value.AccountNumber;
        _accountCurrency = clientOptions.Value.AccountCurrency;
        _environment = clientOptions.Value.UseSandboxApi 
            ? Environment.Mock 
            : Environment.Api;

#if !DEBUG
        if (_environment == Environment.Mock)
            throw new InvalidOperationException($"Mock environment cannot be used in production!");
#endif

        logger.LogInformation($"Using {_environment} as RBCZ environment.");

        X509Certificate2 certificate = new(
            clientOptions.Value.CertificatePath,
            clientOptions.Value.CertificatePassword);

        HttpClientHandler handler = new();
        handler.ClientCertificates.Add(certificate);

        _client = new(new(handler));
    }

    public async Task<ExchangeRate?> GetFxRateAsync(string nonCzkCurrency, CancellationToken ct)
    {
        CurrencyListSimple result = await _client.GetFxRatesAsync(
            _clientId,
            RequestId(),
            _auditIpAddress,
            _environment,
            nonCzkCurrency,
            null,
            ct);

        return result
            .ExchangeRateLists
            .OrderByDescending(l => l.OrdinalNumber)
            .Take(1)
            .SelectMany(l => l.ExchangeRates)
            .FirstOrDefault();
    }

    public async IAsyncEnumerable<Transaction> GetLast90DaysTransactionsAsync([EnumeratorCancellation] CancellationToken ct)
    {
        DateTime today = DateTime.Today;
        DateTime ago90days = today.AddDays(-90);

        int page = 1;
        while (true)
        {
            Response6 response = await _client.GetTransactionListAsync(
                _clientId,
                RequestId(),
                _auditIpAddress,
                _environment,
                _accountNumber,
                _accountCurrency,
                ago90days,
                today,
                page,
                ct);

            foreach (Transaction transaction in response.Transactions)
                yield return transaction;

            if (response.LastPage is true)
                break;

            page++;
        }
    }

    private readonly Client _client;
    private readonly string _clientId;
    private readonly string _auditIpAddress;
    private readonly string _accountNumber;
    private readonly string _accountCurrency;
    private readonly Environment _environment;

    private string RequestId()
        => Guid.NewGuid().ToString();
}