using DSDD.Automations.Hosting.Middleware;
using DSDD.Automations.Payments.Views.Error;
using Microsoft.AspNetCore.Http;
using RazorLight;

namespace DSDD.Automations.Payments.Middleware;

public class PaymentsAdministratorAuthorizationMiddleware : AccessRoleAuthorizationMiddlewareBase
{
    public PaymentsAdministratorAuthorizationMiddleware(IRazorLightEngine engine) : base(ROLE_NAME)
    {
        _engine = engine;
    }

    protected override Task<string?> RenderErrorPage(HttpContext _)
        => _engine.CompileRenderAsync("Error.Error.cshtml", new ErrorViewModel($"Uživatel musí mít roli {ROLE_NAME}!"));

    private const string ROLE_NAME = "payments-administrator";

    private readonly IRazorLightEngine _engine;
}