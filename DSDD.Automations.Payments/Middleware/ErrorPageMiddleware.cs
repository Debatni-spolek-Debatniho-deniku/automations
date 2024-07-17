using DSDD.Automations.Payments.Views.Error;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using RazorLight;
using System.Net.Mime;
using Microsoft.Extensions.Logging;

namespace DSDD.Automations.Payments.Middleware;

public class ErrorPageMiddleware: IFunctionsWorkerMiddleware
{
    public ErrorPageMiddleware(IRazorLightEngine engine, ILogger<ErrorPageMiddleware> logger)
    {
        _engine = engine;
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
                .WriteAsync(await _engine.CompileRenderAsync("Error.Error.cshtml", new ErrorViewModel(ex)));

            _logger.LogError(ex, null);
        }
    }

    private readonly IRazorLightEngine _engine;
    private readonly ILogger<ErrorPageMiddleware> _logger;
}