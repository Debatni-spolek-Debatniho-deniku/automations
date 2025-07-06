using Azure.Core;
using DSDD.Automations.Payments.Persistence.Abstractions;
using DSDD.Automations.Payments.Persistence.Abstractions.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Payments.Persistence.Cosmos;

public class CosmosUnpairedBankPaymentsDao: CosmosDaoBase<UnpairedBankPayment>, IUnpairedBankPaymentsDao
{
    public Task<UnpairedBankPayment?> GetAsync(string paymentReference, CancellationToken ct)
        => GetAsyncInternal(paymentReference, new PartitionKey(paymentReference), ct);

    public IAsyncEnumerable<UnpairedBankPayment> GetAllAsync(CancellationToken ct)
        => GetAllAsyncInternal(ct);

    public Task UpsertAync(UnpairedBankPayment payment, CancellationToken ct)
        => UpsertAyncInternal(payment, ct);
    
    public CosmosUnpairedBankPaymentsDao(TokenCredential tokenCredential, IOptions<CosmosOptions> cosmosOptions, CosmosClient? client) : base(tokenCredential, cosmosOptions, client)
    {
    }

    protected override string ContainerName => "unpairedBankPayments";
}