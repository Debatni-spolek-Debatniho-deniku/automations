using System.Runtime.CompilerServices;
using Azure.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Payments.Persistence.Cosmos;

public abstract class CosmosDaoBase<TDto>: IDisposable
    where TDto : class
{
    public void Dispose()
        => _client?.Dispose();

    protected abstract string ContainerName { get; }

    protected CosmosDaoBase(TokenCredential tokenCredential, IOptions<CosmosOptions> cosmosOptions)
    {
        _tokenCredential = tokenCredential;
        _cosmosOptions = cosmosOptions;
    }

    /// <summary>
    /// Returns <see cref="null"/> if does not exist.
    /// </summary>
    protected async Task<TDto?> GetAsyncInternal(string id, PartitionKey partitionKey, CancellationToken ct)
    {
        Container container = await GetContainerAsync(ct);
        
        try
        {
            return await container.ReadItemAsync<TDto>(id, partitionKey, null, ct);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    protected async IAsyncEnumerable<TDto> GetAllAsyncInternal([EnumeratorCancellation] CancellationToken ct)
    {
        Container container = await GetContainerAsync(ct);

        FeedIterator<TDto> iterator = container.GetItemQueryIterator<TDto>();

        while (iterator.HasMoreResults)
        {
            FeedResponse<TDto> response = await iterator.ReadNextAsync(ct);
            foreach (TDto dto in response.Resource)
                yield return dto;
        }
    }

    protected async Task UpsertAyncInternal(TDto dto, CancellationToken ct)
    {
        Container container = await GetContainerAsync(ct);
        await container.UpsertItemAsync(dto, null, null, ct);
    }

    private readonly TokenCredential _tokenCredential;
    private readonly IOptions<CosmosOptions> _cosmosOptions;

    private CosmosClient? _client;
    
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private const string PAYERS_DATABASE = "payers";

    private async Task<Container> GetContainerAsync(CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);

        if (_client is null)
            _client = await new CosmosClientBuilder(_cosmosOptions.Value.AccountEndpoint, _tokenCredential)
                .WithSerializerOptions(new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase })
                .BuildAndInitializeAsync([(PAYERS_DATABASE, ContainerName)]);

        _semaphore.Release(1);

        return _client
            .GetDatabase(PAYERS_DATABASE)
            .GetContainer(ContainerName);
    }
}