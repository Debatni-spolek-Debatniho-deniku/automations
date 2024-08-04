using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Net.Mime;

namespace DSDD.Automations.Hosting.Middleware;

public abstract class AccessRoleAuthorizationMiddlewareBase: IFunctionsWorkerMiddleware
{
    protected AccessRoleAuthorizationMiddlewareBase(string role)
    {
        _role = role;
    }

    public async Task Invoke(FunctionContext ctx, FunctionExecutionDelegate next)
    {
#if  !DEBUG
        if (ctx.GetHttpContext() is HttpContext httpCtx && !httpCtx.User.IsInRole(_role))
        {
            httpCtx.Response.StatusCode = StatusCodes.Status403Forbidden;

            string? errorPage = await RenderErrorPage();
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

    protected virtual Task<string?> RenderErrorPage()
        => Task.FromResult<string?>(null);

    private readonly string _role;
}