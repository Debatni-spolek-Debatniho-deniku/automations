using System.Net.Mime;
using DSDD.Automations.Payments.Helpers;
using DSDD.Automations.Payments.Model;
using DSDD.Automations.Payments.Views.Index;
using DSDD.Automations.Payments.Views.ManualPayment;
using DSDD.Automations.Payments.Views.Payer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RazorLight;

namespace DSDD.Automations.Payments
{
    public class MvcHttp
    {
        public MvcHttp(IRazorLightEngine engine, IPayers payers, INumericSymbolParser numericSymbolParser, ILogger<MvcHttp> logger)
        {
            _engine = engine;
            _payers = payers;
            _numericSymbolParser = numericSymbolParser;
            _logger = logger;
        }

        [Function(nameof(MvcHttp) + "-" + nameof(GetIndex))]
        public async Task<IActionResult> GetIndex([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payers")] HttpRequest req)
            => new ContentResult()
            {
                Content = await _engine.CompileRenderAsync("Index.Index.cshtml", new IndexViewModel()),
                ContentType = MediaTypeNames.Text.Html,
                StatusCode = StatusCodes.Status200OK
            };

        [Function(nameof(MvcHttp) + "-" + nameof(GetPayer))]
        public async Task<IActionResult> GetPayer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payers/{variableSymbol}")] HttpRequest req,
            string variableSymbol)
        {
            try
            {
                ulong longVariableSymbol = _numericSymbolParser.Parse(variableSymbol);
                Payer? maybePayer = await _payers.GetAsync(longVariableSymbol);
                
                return new ContentResult()
                {
                    Content = await _engine.CompileRenderAsync("Payer.Payer.cshtml", maybePayer is not null
                        ? new PayerViewModel(maybePayer)
                        : new PayerViewModel(longVariableSymbol)),
                    ContentType = MediaTypeNames.Text.Html,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);

                return new ContentResult()
                {
                    Content = await _engine.CompileRenderAsync("Index.Index.cshtml", new IndexViewModel(ex)),
                    ContentType = MediaTypeNames.Text.Html,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        [Function(nameof(MvcHttp) + "-" + nameof(GetNewPayment))]
        public async Task<IActionResult> GetNewPayment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payers/{variableSymbol}/payments/new")] HttpRequest req,
            string variableSymbol)
        {
            ulong longVariableSymbol = _numericSymbolParser.Parse(variableSymbol);

            return new ContentResult()
            {
                Content = await _engine.CompileRenderAsync("ManualPayment.ManualPayment.cshtml", new ManualPaymentViewModel(longVariableSymbol)),
                ContentType = MediaTypeNames.Text.Html,
                StatusCode = StatusCodes.Status200OK
            };
        }

        [Function(nameof(MvcHttp) + "-" + nameof(PostNewPayment))]
        public IActionResult PostNewPayment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "payers/{variableSymbol}/payments/new")] HttpRequest req,
            string variableSymbol,
            [FromForm] ManualPaymentFormViewModel model)
        {
            return new RedirectResult($"/payers/{variableSymbol}");
        }

        private readonly IRazorLightEngine _engine;
        private readonly IPayers _payers;
        private readonly INumericSymbolParser _numericSymbolParser;
        private readonly ILogger<MvcHttp> _logger;
    }
}
