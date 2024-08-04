using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Net.Mime;
using DSDD.Automations.Reports.Razor;
using DSDD.Automations.Reports.Views;
using Microsoft.Extensions.Logging;

namespace DSDD.Automations.Reports.Middleware;

public class ErrorPageMiddleware: IFunctionsWorkerMiddleware
{
    public ErrorPageMiddleware(IRazorRenderer renderer, ILogger<ErrorPageMiddleware> logger)
    {
        _renderer = renderer;
        _logger = logger;
    }

    public async Task Invoke(FunctionContext ctx, FunctionExecutionDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            if (ctx.GetHttpContext() is not HttpContext httpCtx)
                throw;

            httpCtx.Response.ContentType = MediaTypeNames.Text.Html;
            httpCtx.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpCtx
                .Response
                .WriteAsync(await _renderer.RenderAsync(httpCtx, "/Views/Error.cshtml", new ErrorViewModel(ex)));

            _logger.LogError(ex, null);
        }
    }

    private readonly IRazorRenderer _renderer;
    private readonly ILogger<ErrorPageMiddleware> _logger;
}