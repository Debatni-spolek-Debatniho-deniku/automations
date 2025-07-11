﻿using System.Net.Mime;
using DSDD.Automations.Hosting.Durable;
using DSDD.Automations.Hosting.Razor;
using DSDD.Automations.Hosting.SisterApps;
using DSDD.Automations.Payments.Helpers;
using DSDD.Automations.Payments.Payments;
using DSDD.Automations.Payments.Persistence.Abstractions;
using DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;
using DSDD.Automations.Payments.Views;
using DSDD.Automations.Payments.Views.BankPayment;
using DSDD.Automations.Payments.Views.ManualPayment;
using DSDD.Automations.Payments.Views.Payer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DSDD.Automations.Payments;

public class MvcHttp
{
    public MvcHttp(IRazorRenderer rednerer, IPayersDao payers, IPaymentsService paymentsService, 
        INumericSymbolParser numericSymbolParser, IOptions<SisterAppsOptions> sisterAppsOptions,
        ILogger<MvcHttp> logger)
    {
        _rednerer = rednerer;
        _payers = payers;
        _paymentsService = paymentsService;
        _numericSymbolParser = numericSymbolParser;
        _sisterAppsOptions = sisterAppsOptions;
        _logger = logger;
    }

    [Function(nameof(MvcHttp) + "-" + nameof(GetIndex))]
    public async Task<IActionResult> GetIndex([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payers")] HttpRequest req)
        => new ContentResult()
        {
            Content = await _rednerer.RenderAsync(req.HttpContext, "/Views/Index.cshtml", new IndexViewModel(_sisterAppsOptions.Value.SisterAppUrl)),
            ContentType = MediaTypeNames.Text.Html,
            StatusCode = StatusCodes.Status200OK
        };

    [Function(nameof(MvcHttp) + "-" + nameof(GetPayer))]
    public async Task<IActionResult> GetPayer(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payers/{variableSymbol}")] HttpRequest req,
        string variableSymbol)
    {
        ulong longVariableSymbol = _numericSymbolParser.Parse(variableSymbol);
        Payer? maybePayer = await _payers.GetAsync(longVariableSymbol, req.HttpContext.RequestAborted);

        return new ContentResult()
        {
            Content = await _rednerer.RenderAsync(req.HttpContext, "/Views/Payer/Payer.cshtml", maybePayer is not null
                ? new PayerViewModel(maybePayer)
                : new PayerViewModel(longVariableSymbol)),
            ContentType = MediaTypeNames.Text.Html,
            StatusCode = StatusCodes.Status200OK
        };
    }

    [Function(nameof(MvcHttp) + "-" + nameof(GetNewPayment))]
    public async Task<IActionResult> GetNewPayment(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payers/{variableSymbol}/payments/new")] HttpRequest req,
        string variableSymbol)
    {
        ulong longVariableSymbol = _numericSymbolParser.Parse(variableSymbol);

        return new ContentResult()
        {
            Content = await _rednerer.RenderAsync(req.HttpContext, "/Views/ManualPayment/ManualPayment.cshtml", new ManualPaymentViewModel(longVariableSymbol)),
            ContentType = MediaTypeNames.Text.Html,
            StatusCode = StatusCodes.Status200OK
        };
    }

    [Function(nameof(MvcHttp) + "-" + nameof(PostNewPayment))]
    public async Task<IActionResult> PostNewPayment(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "payers/{variableSymbol}/payments/new")] HttpRequest req,
        string variableSymbol)
    {
        ManualPaymentFormViewModel model = BindToManualPaymentFormViewModel(await req.ReadFormAsync());

        await _paymentsService.UpsertManualPaymentAsync(
            _numericSymbolParser.Parse(variableSymbol),
            null,
            model.ConstantSymbol,
            model.AmountCzk,
            model.DateTime,
            model.Description,
            req.HttpContext.RequestAborted);

        return new RedirectResult($"/payers/{variableSymbol}");
    }

    [Function(nameof(MvcHttp) + "-" + nameof(GetPayment))]
    public async Task<IActionResult> GetPayment(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payers/{variableSymbol}/payments/{paymentReference}")] HttpRequest req,
        string variableSymbol,
        string paymentReference)
    {
        return new ContentResult()
        {
            Content = await GetContent(),
            ContentType = MediaTypeNames.Text.Html,
            StatusCode = StatusCodes.Status200OK
        };

        async Task<string> GetContent()
        {
            ulong longVariableSymbol = _numericSymbolParser.Parse(variableSymbol);
            Payer payer = await _payers.GetRequiredAsync(longVariableSymbol, req.HttpContext.RequestAborted);

            switch (await _paymentsService.GetPaymentAsync(longVariableSymbol, paymentReference, req.HttpContext.RequestAborted))
            {
                case PayerBankPayment bankPayment:
                    return await _rednerer.RenderAsync(
                        req.HttpContext,
                        "/Views/BankPayment/BankPayment.cshtml",
                        new BankPaymentViewModel(longVariableSymbol, bankPayment));
                case PayerManualPayment manualPayment:
                    return await _rednerer.RenderAsync(
                        req.HttpContext,
                        "/Views/ManualPayment/ManualPayment.cshtml",
                        new ManualPaymentViewModel(longVariableSymbol, manualPayment));
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }

    [Function(nameof(MvcHttp) + "-" + nameof(PostPayment))]
    public async Task<IActionResult> PostPayment(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "payers/{variableSymbol}/payments/{paymentReference}")] HttpRequest req,
        string variableSymbol,
        string paymentReference)
    {
        ulong longVariableSymbol = _numericSymbolParser.Parse(variableSymbol);

        IFormCollection form = await req.ReadFormAsync();

        bool isBank = (await _paymentsService.GetPaymentAsync(
            longVariableSymbol, 
            paymentReference, 
            req.HttpContext.RequestAborted)) is PayerBankPayment;

        if (isBank)
        {
            BankPaymentFormViewModel model = BindToBankPaymentFormViewModel(form);
            await _paymentsService.OverrideBankPaymentAsync(
                longVariableSymbol,
                paymentReference,
                model.ConstantSymbol,
                model.DateTime,
                model.Description,
                req.HttpContext.RequestAborted);
        }
        else
        {
            ManualPaymentFormViewModel model = BindToManualPaymentFormViewModel(form);
            await _paymentsService.UpsertManualPaymentAsync(
                longVariableSymbol,
                paymentReference,
                model.ConstantSymbol,
                model.AmountCzk,
                model.DateTime,
                model.Description,
                req.HttpContext.RequestAborted);
        }

        return new RedirectResult($"/payers/{variableSymbol}");
    }

    [Function(nameof(MvcHttp) + "-" + nameof(PostRemovePayment))]
    public async Task<IActionResult> PostRemovePayment(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "payers/{variableSymbol}/payments/{paymentReference}/remove")] HttpRequest req,
        string variableSymbol,
        string paymentReference)
    {
        await _paymentsService.RemovePaymentAsync(
            _numericSymbolParser.Parse(variableSymbol), 
            paymentReference,
            req.HttpContext.RequestAborted);

        return new RedirectResult($"/payers/{variableSymbol}");
    }

    [Function(nameof(MvcHttp) + "-" + nameof(PostRestorePayment))]
    public async Task<IActionResult> PostRestorePayment(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "payers/{variableSymbol}/payments/{paymentReference}/restore")] HttpRequest req,
        string variableSymbol,
        string paymentReference)
    {
        await _paymentsService.RestorePaymentAsync(
            _numericSymbolParser.Parse(variableSymbol),
            paymentReference,
            req.HttpContext.RequestAborted);

        return new RedirectResult($"/payers/{variableSymbol}");
    }

    [Function(nameof(MvcHttp) + "-" + nameof(PostImportPayments))]
    public async Task<IActionResult> PostImportPayments(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "import-payments")] HttpRequest req,
        [DurableClient] DurableTaskClient durableClient,
        string variableSymbol)
    {
        _logger.LogInformation("User {User} started bank payments import.", req.HttpContext.User.Identity?.Name ?? "UNKNOWN");

        string orchestrationId = await durableClient.ScheduleNewMethodOrchestrationInstanceAsync<RbczImport>(_ => _.Orhcestration(default!));

        return new ContentResult()
        {
            Content = await _rednerer.RenderAsync(
                req.HttpContext, 
                "/Views/Callout.cshtml",
                new CalloutViewModel("Úspěch", $"Import plateb z banky byl úspěšně spuštěn jako orchestrace s ID={orchestrationId}. Platby budou u jednotlivých poplatníků zobrazeny, až import skončí. To může trvat i několik minut.", CalloutBackButtonColor.SUCCESS)),
            ContentType = MediaTypeNames.Text.Html,
            StatusCode = StatusCodes.Status200OK
        };
    }

    private readonly IRazorRenderer _rednerer;
    private readonly IPayersDao _payers;
    private readonly IPaymentsService _paymentsService;
    private readonly INumericSymbolParser _numericSymbolParser;
    private readonly IOptions<SisterAppsOptions> _sisterAppsOptions;
    private readonly ILogger<MvcHttp> _logger;

    private ManualPaymentFormViewModel BindToManualPaymentFormViewModel(IFormCollection form)
    {
        ulong? constantSymbol = form[nameof(ManualPaymentFormViewModel.ConstantSymbol)].FirstOrDefault() is { } c 
                                && !string.IsNullOrWhiteSpace(c)
            ? _numericSymbolParser.Parse(c)
            : null;
        decimal amount = decimal.Parse(form[nameof(ManualPaymentFormViewModel.AmountCzk)].FirstOrDefault()!);
        DateTime dateTime = DateTime.Parse(form[nameof(ManualPaymentFormViewModel.DateTime)].FirstOrDefault()!);
        string? description = form[nameof(ManualPaymentFormViewModel.Description)].FirstOrDefault() is { } d &&
                              !string.IsNullOrWhiteSpace(d)
            ? d
            : null;

        return new(constantSymbol, amount, dateTime, description);
    }

    private BankPaymentFormViewModel BindToBankPaymentFormViewModel(IFormCollection form)
    {
        ulong? constantSymbol = form[nameof(BankPaymentFormViewModel.ConstantSymbol)].FirstOrDefault() is { } c 
                                && !string.IsNullOrWhiteSpace(c)
            ? _numericSymbolParser.Parse(c)
            : null;
        DateTime? dateTime = form[nameof(BankPaymentFormViewModel.DateTime)].FirstOrDefault() is {} dt
                             && !string.IsNullOrWhiteSpace(dt)
            ? DateTime.Parse(dt)
            : null; 
        string? description = form[nameof(BankPaymentFormViewModel.Description)].FirstOrDefault() is { } d 
                              && !string.IsNullOrWhiteSpace(d)
            ? d
            : null;

        return new(constantSymbol, dateTime, description);
    }
}
