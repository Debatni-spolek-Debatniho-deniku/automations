using Azure.Core;
using DSDD.Automations.Payments.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Payments;

public class CosmosPayersDao: IPayersDao, IDisposable
{
    public CosmosPayersDao(TokenCredential tokenCredential, IOptions<CosmosOptions> cosmosOptions)
    {
        _tokenCredential = tokenCredential;
        _accountEndpoint = cosmosOptions.Value.AccountEndpont;
    }

    /// <summary>
    /// Returns <see cref="null"/> if does not exist.
    /// </summary>
    public async Task<Payer?> GetAsync(ulong variableSymbol, CancellationToken ct)
    {
        Container payers = await GetPayersContainer(ct);

        string id = variableSymbol.ToString();

        try
        {
            return await payers.ReadItemAsync<Payer>(id, new PartitionKey(id), null, ct);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task UpsertAync(Payer payer, CancellationToken ct)
    {
        Container payers = await GetPayersContainer(ct);
        await payers.UpsertItemAsync(payer, null, null, ct);
    }
    
    public void Dispose()
        => _client?.Dispose();

    private CosmosClient? _client;
    private readonly TokenCredential _tokenCredential;
    private readonly string _accountEndpoint;
    private readonly SemaphoreSlim _semaphore = new(1,1);

    private const string PAYERS_DATABASE = "payers";
    private const string PAYERS_CONTAINER = "payers";

    private async Task<Container> GetPayersContainer(CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);

        if (_client is null)
            _client = await new CosmosClientBuilder(_accountEndpoint, _tokenCredential)
                .WithSerializerOptions(new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase })
                .BuildAndInitializeAsync([(PAYERS_DATABASE, PAYERS_CONTAINER)]);

        _semaphore.Release(1);

        return _client
            .GetDatabase(PAYERS_DATABASE)
            .GetContainer(PAYERS_CONTAINER);
    }
}