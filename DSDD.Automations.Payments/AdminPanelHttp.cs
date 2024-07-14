using System.Dynamic;
using System.Net.Mime;
using DSDD.Automations.Payments.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RazorLight;

namespace DSDD.Automations.Payments
{
    public class AdminPanelHttp
    {
        public AdminPanelHttp(IRazorLightEngine engine, ILogger<AdminPanelHttp> logger)
        {
            _engine = engine;
            _logger = logger;
        }

        [Function(nameof(AdminPanelHttp) + "-" + nameof(GetIndex))]
        public async Task<IActionResult> GetIndex([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payers")] HttpRequest req)
            => new ContentResult()
            {
                Content = await _engine.CompileRenderAsync("Index.cshtml", new IndexViewModel("Payers")),
                ContentType = MediaTypeNames.Text.Html,
                StatusCode = StatusCodes.Status200OK
            };

        //[Function(nameof(AdminPanelHttp) + "-" + nameof(GetPayer))]
        //public IActionResult GetPayer([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{variableSymbol}")] HttpRequest req, string variableSymbol)
        //{
        //    _logger.LogInformation("C# HTTP trigger function processed a request.");
        //    return new OkObjectResult("Welcome to Azure Functions!");
        //}

        private readonly IRazorLightEngine _engine;
        private readonly ILogger<AdminPanelHttp> _logger;
    }
}
