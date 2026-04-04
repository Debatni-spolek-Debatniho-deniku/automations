using System.Text.Json;
using System.Text.Json.Serialization;
using DSDD.Automations.Reports.Members;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Memory;

namespace DSDD.Automations.Members;

public class MembersHttp
{
    public MembersHttp(IMembersProvider membersProvider, IMemoryCache cache)
    {
        _membersProvider = membersProvider;
        _cache = cache;
    }

    [Function(nameof(MembersHttp) + "-" + nameof(GetMembers))]
    public async Task<IActionResult> GetMembers(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/members")] HttpRequest req,
        CancellationToken ct)
    {
        IReadOnlyCollection<Member> members = await _cache.GetOrCreateAsync(
            CacheKey,
            async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                return await _membersProvider.GetMembersAsync(ct);
            }) ?? await _membersProvider.GetMembersAsync(ct);

        return new JsonResult(members, _jsonOptions);
    }

    private readonly IMembersProvider _membersProvider;
    private readonly IMemoryCache _cache;

    private const string CacheKey = "members";

    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };
}
