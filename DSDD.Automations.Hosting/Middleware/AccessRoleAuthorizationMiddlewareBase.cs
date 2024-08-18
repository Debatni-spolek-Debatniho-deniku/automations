using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using DSDD.Automations.Hosting.Razor;
#if !DEBUG
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
#endif

namespace DSDD.Automations.Hosting.Middleware;

public abstract class AccessRoleAuthorizationMiddlewareBase<TModel>: IFunctionsWorkerMiddleware
{
    protected AccessRoleAuthorizationMiddlewareBase(IRazorRenderer renderer)
    {
        _renderer = renderer;
    }

    public async Task Invoke(FunctionContext ctx, FunctionExecutionDelegate next)
    {
#if !DEBUG
        if (ctx.GetHttpContext() is HttpContext httpCtx && !httpCtx.User.IsInRole(Role))
        {
            httpCtx.Response.StatusCode = StatusCodes.Status403Forbidden;

            string? errorPage = await _renderer.RenderAsync(httpCtx, ViewPath, CreateModel($"Uživatle musí mít roli {Role}!"));
            if (!string.IsNullOrWhiteSpace(errorPage))
            {
                httpCtx.Response.ContentType = MediaTypeNames.Text.Html;
                await httpCtx
                    .Response
                    .WriteAsync(errorPage, httpCtx.RequestAborted);
            }

            return;
        }  
#endif


        await next(ctx);
    }

    protected abstract string Role { get; }

    protected abstract string ViewPath { get; }

    protected abstract TModel CreateModel(string message);

    private readonly IRazorRenderer _renderer;
}