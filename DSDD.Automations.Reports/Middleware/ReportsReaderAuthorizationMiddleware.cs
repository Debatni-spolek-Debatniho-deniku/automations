using DSDD.Automations.Hosting.Middleware;
using DSDD.Automations.Hosting.Razor;
using DSDD.Automations.Reports.Views;
using Microsoft.AspNetCore.Http;

namespace DSDD.Automations.Reports.Middleware;

public class ReportsReaderAuthorizationMiddleware: AccessRoleAuthorizationMiddlewareBase
{
    public ReportsReaderAuthorizationMiddleware(IRazorRenderer renderer) : base(ROLE_NAME)
    {
        _renderer = renderer;
    }

    private const string ROLE_NAME = "reports-reader";

    protected override async Task<string?> RenderErrorPage(HttpContext ctx)
        => await _renderer.RenderAsync(ctx, "/Views/Error.cshtml", new ErrorViewModel($"Uživatel musí mít roli {ROLE_NAME}!"));

    private readonly IRazorRenderer _renderer;
}