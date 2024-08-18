using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace DSDD.Automations.Hosting.Razor;

public class RazorRenderer : IRazorRenderer
{
    public RazorRenderer(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> RenderAsync<TModel>(HttpContext ctx, string viewName, TModel model)
    {
        ActionContext actionCtx = new(ctx, new RouteData(), new ActionDescriptor());
        IView view = FindView(actionCtx, viewName);

        ViewDataDictionary<TModel> data = new(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };

        StringWriter sw = new();

        ViewContext viewCtx = new(
            actionCtx,
            FindView(actionCtx, viewName),
            new(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            },
            new TempDataDictionary(actionCtx.HttpContext, _tempDataProvider),
            sw,
            new HtmlHelperOptions());

        await view.RenderAsync(viewCtx);

        return sw.ToString();
    }

    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    private IView FindView(ActionContext actionContext, string viewName)
    {
        ViewEngineResult? getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
        if (getViewResult.Success) return getViewResult.View;

        ViewEngineResult? findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
        if (findViewResult.Success) return findViewResult.View;

        throw new InvalidOperationException(string.Join(
            Environment.NewLine,
            new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(
                getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations))));
    }
}