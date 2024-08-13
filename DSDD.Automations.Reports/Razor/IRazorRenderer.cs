using Microsoft.AspNetCore.Http;

namespace DSDD.Automations.Reports.Razor;

public interface IRazorRenderer
{
    Task<string> RenderAsync<TModel>(HttpContext ctx, string viewName, TModel model);
}