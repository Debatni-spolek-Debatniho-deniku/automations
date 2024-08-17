using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace DSDD.Automations.Hosting.Middleware;

public class ClaimsPrincipalPoplulatingMiddleware: IFunctionsWorkerMiddleware
{
    public Task Invoke(FunctionContext ctx, FunctionExecutionDelegate next)
    {
        if (ctx.GetHttpContext() is HttpContext httpCtx)
            httpCtx.User = Parse(httpCtx.Request);

        return next(ctx);
    }

    /// <summary>
    /// For some strange reason role_typ claim name is not the same claim name used for App roles.
    /// https://learn.microsoft.com/en-us/entra/external-id/customers/how-to-use-app-roles-customers
    /// </summary>
    private const string APP_ROLES_CLAIM_TYPE = "roles";

    private class ClientPrincipalClaim
    {
        [JsonPropertyName("typ")]
        public string Type { get; }
        [JsonPropertyName("val")]
        public string Value { get; }

        public ClientPrincipalClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    private class ClientPrincipal
    {
        [JsonPropertyName("auth_typ")]
        public string IdentityProvider { get; }
        [JsonPropertyName("name_typ")]
        public string NameClaimType { get; }
        [JsonPropertyName("role_typ")]
        public string RoleClaimType { get; }
        [JsonPropertyName("claims")]
        public IEnumerable<ClientPrincipalClaim> Claims { get; }

        public ClientPrincipal(string identityProvider, string nameClaimType, string roleClaimType, IEnumerable<ClientPrincipalClaim> claims)
        {
            IdentityProvider = identityProvider;
            NameClaimType = nameClaimType;
            RoleClaimType = roleClaimType;
            Claims = claims;
        }
    }

    /// <summary>
    /// Code below originally from Microsoft Docs - https://learn.microsoft.com/en-us/azure/app-service/configure-authentication-user-identities
    /// </summary>
    public ClaimsPrincipal Parse(HttpRequest req)
    {
        ClientPrincipal? principal = null;

        if (req.Headers.TryGetValue("x-ms-client-principal", out var header))
        {
            var data = header.First();
            var decoded = Convert.FromBase64String(data!);
            var json = Encoding.UTF8.GetString(decoded);

            principal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        var identity = new ClaimsIdentity(principal?.IdentityProvider, principal?.NameClaimType, APP_ROLES_CLAIM_TYPE);
        identity.AddClaims(principal?.Claims.Select(c => new Claim(
            c.Type == principal.RoleClaimType
                // Unify multiple role claim names. 
                ? APP_ROLES_CLAIM_TYPE
                : c.Type,
            c.Value)) ?? Enumerable.Empty<Claim>());

        return new ClaimsPrincipal(identity);
    }
}