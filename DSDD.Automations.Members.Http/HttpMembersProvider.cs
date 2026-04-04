using System.Text.Json;
using System.Text.Json.Serialization;
using DSDD.Automations.Reports.Members;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Members.Http;

public class HttpMembersProvider : IMembersProvider
{
    public HttpMembersProvider(HttpClient httpClient, IOptions<HttpMembersProviderOptions> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = options.Value.BaseUrl;
        _httpClient.DefaultRequestHeaders.Add("x-functions-key", options.Value.FunctionKey);
    }

    public async Task<IReadOnlyCollection<Member>> GetMembersAsync(CancellationToken ct)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync("api/members", ct);
        response.EnsureSuccessStatusCode();

        await using Stream stream = await response.Content.ReadAsStreamAsync(ct);
        MemberDto[] dtos = await JsonSerializer.DeserializeAsync<MemberDto[]>(stream, _jsonOptions, ct)
                           ?? Array.Empty<MemberDto>();

        return dtos
            .Select(d => new Member(d.FirstName, d.LastName, d.VariableSymbol, d.Enlisted, d.FeePaymentPeriod))
            .ToArray();
    }

    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private record MemberDto(
        string FirstName,
        string LastName,
        ulong VariableSymbol,
        DateOnly Enlisted,
        MemberFeePaymentPeriod FeePaymentPeriod);
}
