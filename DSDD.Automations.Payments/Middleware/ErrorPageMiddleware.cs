using DSDD.Automations.Hosting.Middleware;
using DSDD.Automations.Hosting.Razor;
using DSDD.Automations.Payments.Views;
using Microsoft.Extensions.Logging;

namespace DSDD.Automations.Payments.Middleware;

public class ErrorPageMiddleware: ErrorPageMiddlewareBase<CalloutViewModel>
{
    public ErrorPageMiddleware(IRazorRenderer renderer, ILogger<ErrorPageMiddlewareBase<CalloutViewModel>> logger) : base(renderer, logger)
    {
    }

    protected override string ViewPath => "/Views/Callout.cshtml";

    protected override CalloutViewModel CreateModel(Exception ex)
        => CalloutViewModel.Error(ex.Message);

}