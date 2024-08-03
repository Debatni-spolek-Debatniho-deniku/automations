using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace DSDD.Automations.Payments.Middleware;

public class AuthorizationMiddleware: IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext ctx, FunctionExecutionDelegate next)
    {
#if !DEBUG
        if (ctx.GetHttpContext() is HttpContext httpCtx)
        {
            ClaimsPrincipal principal = Parse(httpCtx.Request);
            if (!principal.IsInRole(ROLE_NAME))
            {
                httpCtx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }
#endif

        await next(ctx);
    }

    private const string ROLE_NAME = "payments-administrator";

    private class ClientPrincipal
    {
        public string IdentityProvider { get; set; } = "";

        public string UserId { get; set; } = "";

        public string UserDetails { get; set; } = "";

        public IEnumerable<string>? UserRoles { get; set; }
    }

    /// <summary>
    /// Code below originally from Microsoft Docs - https://docs.microsoft.com/en-gb/azure/static-web-apps/user-information?tabs=csharp#api-functions
    /// </summary>
    public static ClaimsPrincipal Parse(HttpRequest req)
    {
        var principal = new ClientPrincipal();

        if (req.Headers.TryGetValue("x-ms-client-principal", out var header))
        {
            var data = header.First();
            var decoded = Convert.FromBase64String(data);
            var json = Encoding.UTF8.GetString(decoded);
            principal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        principal.UserRoles = principal.UserRoles?.Except(new[] { "anonymous" }, StringComparer.CurrentCultureIgnoreCase);

        if (!principal.UserRoles?.Any() ?? true)
        {
            return new ClaimsPrincipal();
        }

        var identity = new ClaimsIdentity(principal.IdentityProvider);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId));
        identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails));
        identity.AddClaims(principal.UserRoles.Select(r => new Claim(ClaimTypes.Role, r)));

        return new ClaimsPrincipal(identity);
    }
}