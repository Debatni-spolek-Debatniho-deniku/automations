using System.Net.Mime;
using DSDD.Automations.Hosting.Razor;
using DSDD.Automations.Hosting.SisterApps;
using DSDD.Automations.Payments.Helpers;
using DSDD.Automations.Reports.Reports;
using DSDD.Automations.Reports.Views;
using DSDD.Automations.Reports.Views.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Reports;

public class MvcHttp
{
    public MvcHttp(IOptions<SisterAppsOptions> sisterAppsOptions, IRazorRenderer renderer, INumericSymbolParser numericSymbolParser, 
        IPayedTotalReport payedTotalReport, IPayerPaymentsReport payerPaymentsReport, IMemberFeesReport memberFeesReport)
    {
        _sisterAppsOptions = sisterAppsOptions;
        _renderer = renderer;
        _numericSymbolParser = numericSymbolParser;
        _payedTotalReport = payedTotalReport;
        _payerPaymentsReport = payerPaymentsReport;
        _memberFeesReport = memberFeesReport;
    }

    [Function(nameof(MvcHttp) + "-" + nameof(GetIndex))]
    public async Task<IActionResult> GetIndex([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "reports")] HttpRequest req)
        => new ContentResult()
        {
            Content = await _renderer.RenderAsync(req.HttpContext, "/Views/Index.cshtml", new IndexViewModel(_sisterAppsOptions.Value.SisterAppUrl)),
            ContentType = MediaTypeNames.Text.Html,
            StatusCode = StatusCodes.Status200OK
        };

    [Function(nameof(MvcHttp) + "-" + nameof(PostPayerPayments))]
    public async Task<IActionResult> PostPayerPayments([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reports/payer-payments")] HttpRequest req, CancellationToken ct)
    {
        PayerPaymentsFormViewModel model = BindToPayerPaymentsFormViewModel(await req.ReadFormAsync());
        Stream xlsx = await _payerPaymentsReport.GenerateXlsxAsync(model.VariableSymbol, model.ConstantSymbol, ct);
        return CreateXlsxResult(xlsx, $"payments-{model.VariableSymbol}{(model.ConstantSymbol is ulong cs ? $"-on-{cs}" : "")}");
    }

    [Function(nameof(MvcHttp) + "-" + nameof(PostPayedTotal))]
    public async Task<IActionResult> PostPayedTotal([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reports/payed-total")] HttpRequest req, CancellationToken ct)
    {
        PayedTotalFormViewModel model = BindToPayedTotalFormViewModel(await req.ReadFormAsync());
        Stream xlsx = await _payedTotalReport.GenerateXlsxAsync(model.ConstantSymbol, ct);
        return CreateXlsxResult(xlsx, $"payer-total-{model.ConstantSymbol}");
    }

    [Function(nameof(MvcHttp) + "-" + nameof(PostMemberFees))]
    public async Task<IActionResult> PostMemberFees([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reports/member-fees")] HttpRequest req, CancellationToken ct)
    {
        Stream xlsx = await _memberFeesReport.GenerateXlsxAsync(ct);
        return CreateXlsxResult(xlsx, "member-fees");
    }

    private readonly IOptions<SisterAppsOptions> _sisterAppsOptions;

    private readonly IRazorRenderer _renderer;
    private readonly INumericSymbolParser _numericSymbolParser;

    private readonly IPayedTotalReport _payedTotalReport;
    private readonly IPayerPaymentsReport _payerPaymentsReport;
    private readonly IMemberFeesReport _memberFeesReport;

    private PayerPaymentsFormViewModel BindToPayerPaymentsFormViewModel(IFormCollection form)
    {
        ulong variableSymbol =
            _numericSymbolParser.Parse(form[nameof(PayerPaymentsFormViewModel.VariableSymbol)].FirstOrDefault()!);

        ulong? constantSymbol = form[nameof(PayerPaymentsFormViewModel.ConstantSymbol)].FirstOrDefault() is { } c
                                && !string.IsNullOrWhiteSpace(c)
            ? _numericSymbolParser.Parse(c)
            : null;

        return new(variableSymbol, constantSymbol);
    }

    private PayedTotalFormViewModel BindToPayedTotalFormViewModel(IFormCollection form)
    {
        ulong constantSymbol =
            _numericSymbolParser.Parse(form[nameof(PayedTotalFormViewModel.ConstantSymbol)].FirstOrDefault()!);

        return new(constantSymbol);
    }

    private IActionResult CreateXlsxResult(Stream stream, string name)
        => new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = $"{name}-{DateTime.Now:s}.xlsx"
        };
}