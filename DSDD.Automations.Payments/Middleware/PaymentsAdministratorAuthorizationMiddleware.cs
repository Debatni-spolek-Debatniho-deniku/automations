using DSDD.Automations.Hosting.Middleware;
using DSDD.Automations.Hosting.Razor;
using DSDD.Automations.Payments.Views;

namespace DSDD.Automations.Payments.Middleware;

public class PaymentsAdministratorAuthorizationMiddleware : AccessRoleAuthorizationMiddlewareBase<ErrorViewModel>
{
    public PaymentsAdministratorAuthorizationMiddleware(IRazorRenderer renderer) : base(renderer)
    {
    }

    protected override string Role => "payments-administrator";

    protected override string ViewPath => "/Views/Error.cshtml";

    protected override ErrorViewModel CreateModel(string message)
        => new ErrorViewModel(message);
}