using DSDD.Automations.Hosting.Razor;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Net.Mime;

namespace DSDD.Automations.Hosting.Middleware;

public abstract class ErrorPageMiddlewareBase<TModel>: IFunctionsWorkerMiddleware
{
    protected ErrorPageMiddlewareBase(IRazorRenderer renderer, ILogger<ErrorPageMiddlewareBase<TModel>> logger)
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
                .WriteAsync(await _renderer.RenderAsync(httpCtx, ViewPath, CreateModel(ex)));

            _logger.LogError(ex, "Error");
        }
    }

    protected abstract string ViewPath { get; }

    protected abstract TModel CreateModel(Exception ex);

    private readonly IRazorRenderer _renderer;
    private readonly ILogger<ErrorPageMiddlewareBase<TModel>> _logger;
}