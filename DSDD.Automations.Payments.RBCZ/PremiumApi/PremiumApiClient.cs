using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using DSDD.Automations.Payments.RBCZ.Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Payments.RBCZ.PremiumApi;

internal class PremiumApiClient: IPremiumApiClient
{   
    public PremiumApiClient(
        IAccountCertificateProvider certificateProvider, 
        IOptions<PremiumApiClientOptions> clientOptions,
        ILogger<PremiumApiClient> logger)
    {
        _certificateProvider = certificateProvider;

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
    }

    public async Task<ExchangeRate?> GetFxRateAsync(string nonCzkCurrency, CancellationToken ct)
    {
        Client client = await GetClientAsync(ct);
        CurrencyListSimple result = await client.GetFxRatesAsync(
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
        // Must be in format 2021-08-01T10:00:00.0Z
        DateTime today = DateTime.UtcNow.Date;
        DateTime.SpecifyKind(today, DateTimeKind.Utc);

        // If added 90, api would throw as it "adds" extra day. Is this a bug or a feature?
        DateTime ago90days = today.AddDays(-89);

        int page = 1;
        while (true)
        {
            Client client = await GetClientAsync(ct);
            Response6 response = await client.GetTransactionListAsync(
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

    private Client? _client;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly IAccountCertificateProvider _certificateProvider;

    private readonly string _clientId;
    private readonly string _auditIpAddress;
    private readonly string _accountNumber;
    private readonly string _accountCurrency;
    private readonly Environment _environment;

    private string RequestId()
        => Guid.NewGuid().ToString();

    private async Task<Client> GetClientAsync(CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);

        if (_client is null)
        {
            X509Certificate certificate = await _certificateProvider.GetAsync(ct);

            HttpClientHandler handler = new();
            handler.ClientCertificates.Add(certificate);

            _client = new(new(handler));
        }

        _semaphore.Release(1);
        
        return _client;
    }
}