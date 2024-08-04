using System.Net.Mime;
using DSDD.Automations.Reports.Members;
using DSDD.Automations.Reports.Razor;
using DSDD.Automations.Reports.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace DSDD.Automations.Reports;

public class MvcHttp
{
    public MvcHttp(IRazorRenderer renderer)
    {
        _renderer = renderer;
    }

    [Function(nameof(MvcHttp) + "-" + nameof(GetIndex))]
    public async Task<IActionResult> GetIndex([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "reports")] HttpRequest req)
        => new ContentResult()
        {
            Content = await _renderer.RenderAsync(req.HttpContext, "/Views/Index.cshtml", new IndexViewModel()),
            ContentType = MediaTypeNames.Text.Html,
            StatusCode = StatusCodes.Status200OK
        };
    
    private readonly IRazorRenderer _renderer;
}