using Azure.Core;
using DSDD.Automations.Payments.Persistence.Abstractions;
using DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Payments.Persistence.Cosmos;

public class CosmosPayersDao: CosmosDaoBase<Payer>, IPayersDao
{
    public CosmosPayersDao(TokenCredential tokenCredential, IOptions<CosmosOptions> cosmosOptions) : base(tokenCredential, cosmosOptions)
    {
    }


    /// <inheritdoc cref="CosmosDaoBase{Payer}.GetAsyncInternal"/>
    public Task<Payer?> GetAsync(ulong variableSymbol, CancellationToken ct)
    {
        string id = variableSymbol.ToString();

        return GetAsyncInternal(id, new PartitionKey(id), ct);
    }

    public IAsyncEnumerable<Payer> GetAllAsync(CancellationToken ct)
        => GetAllAsyncInternal(ct);

    public Task UpsertAync(Payer payer, CancellationToken ct)
        => UpsertAyncInternal(payer, ct);

    protected override string ContainerName => "payers";
}