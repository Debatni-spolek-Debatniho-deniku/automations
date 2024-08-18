using DSDD.Automations.Hosting.Razor;
using DSDD.Automations.Reports.Views;
using Microsoft.Extensions.Logging;
using DSDD.Automations.Hosting.Middleware;

namespace DSDD.Automations.Reports.Middleware;

public class ErrorPageMiddleware: ErrorPageMiddlewareBase<ErrorViewModel>
{
    public ErrorPageMiddleware(IRazorRenderer renderer, ILogger<ErrorPageMiddlewareBase<ErrorViewModel>> logger) : base(renderer, logger)
    {
    }

    protected override string ViewPath => "/Views/Error.cshtml";

    protected override ErrorViewModel CreateModel(Exception ex)
        => new ErrorViewModel(ex);
}