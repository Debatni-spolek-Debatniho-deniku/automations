using Azure.Core;
using DSDD.Automations.Payments.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace DSDD.Automations.Payments;

public class CosmosPayers: IPayers, IDisposable
{
    public CosmosPayers(TokenCredential tokenCredential, string accountEndpoint)
    {
        _tokenCredential = tokenCredential;
        _accountEndpoint = accountEndpoint;
    }

    /// <summary>
    /// Returns <see cref="null"/> if does not exist.
    /// </summary>
    public async Task<Payer?> GetAsync(ulong variableSymbol)
    {
        Container payers = await GetPayersContainer();

        try
        {
            return await payers.ReadItemAsync<Payer>(
                variableSymbol.ToString(),
                new PartitionKey(variableSymbol));
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task UpsertAync(Payer payer)
    {
        Container payers = await GetPayersContainer();;
        await payers.UpsertItemAsync(payer);
    }
    
    public void Dispose()
        => _client?.Dispose();

    private CosmosClient? _client;
    private readonly TokenCredential _tokenCredential;
    private readonly string _accountEndpoint;
    private readonly SemaphoreSlim _semaphore = new(1,1);

    private const string PAYERS_DATABASE = "payers";
    private const string PAYERS_CONTAINER = "payers";

    private async Task<Container> GetPayersContainer()
    {
        await _semaphore.WaitAsync();

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