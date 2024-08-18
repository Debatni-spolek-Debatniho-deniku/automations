using DSDD.Automations.Hosting.Middleware;
using DSDD.Automations.Hosting.Razor;
using DSDD.Automations.Reports.Views;

namespace DSDD.Automations.Reports.Middleware;

public class ReportsReaderAuthorizationMiddleware: AccessRoleAuthorizationMiddlewareBase<ErrorViewModel>
{
    public ReportsReaderAuthorizationMiddleware(IRazorRenderer renderer) : base(renderer)
    {
    }

    protected override string Role => "payments-administrator";

    protected override string ViewPath => "/Views/Error.cshtml";

    protected override ErrorViewModel CreateModel(string message)
        => new ErrorViewModel(message);
}