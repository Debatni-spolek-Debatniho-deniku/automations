using DSDD.Automations.Hosting.Middleware;
using DSDD.Automations.Hosting.Razor;
using DSDD.Automations.Payments.Views;

namespace DSDD.Automations.Payments.Middleware;

public class PaymentsAdministratorAuthorizationMiddleware : AccessRoleAuthorizationMiddlewareBase<CalloutViewModel>
{
    public PaymentsAdministratorAuthorizationMiddleware(IRazorRenderer renderer) : base(renderer)
    {
    }

    protected override string Role => "payments-administrator";

    protected override string ViewPath => "/Views/Callout.cshtml";

    protected override CalloutViewModel CreateModel(string message)
        => CalloutViewModel.Error(message);
}