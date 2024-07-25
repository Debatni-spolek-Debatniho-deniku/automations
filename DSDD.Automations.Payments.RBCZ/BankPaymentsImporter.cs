using DSDD.Automations.Payments.Helpers;
using DSDD.Automations.Payments.Model;
using DSDD.Automations.Payments.RBCZ.PremiumApi;
using Microsoft.Extensions.Logging;
using System;

namespace DSDD.Automations.Payments.RBCZ;

internal class BankPaymentsImporter: IBankPaymentsImporter
{
    public BankPaymentsImporter(ILogger<BankPaymentsImporter> logger, IPayersDao payers, IPremiumApiClient client, INumericSymbolParser numericSymbolParser, IFxRates fxRates)
    {
        _logger = logger;
        _payers = payers;
        _client = client;
        _numericSymbolParser = numericSymbolParser;
        _fxRates = fxRates;
    }

    public async Task ImportAsync(CancellationToken ct)
    {
        _logger.LogInformation("Importing RBCZ payments.");

        ILookup<ulong, Transaction> byVaraibleSymbol = await GetTransactionsAsync(ct);

        foreach (IGrouping<ulong, Transaction> group in byVaraibleSymbol)
        {
            _logger.LogInformation("Working on varaible symbol {VariableSymbol}", group.Key);

            Payer? payer = await _payers.GetAsync(group.Key, ct);
            payer ??= new(group.Key);

            await MergePaymentsAsync(payer.BankPayments, group, ct);

            await _payers.UpsertAync(payer, ct);
        }

        _logger.LogInformation("Finished importing RBCZ payments.");
    }

    private readonly ILogger<BankPaymentsImporter> _logger;
    private readonly IPayersDao _payers;
    private readonly IPremiumApiClient _client;
    private readonly INumericSymbolParser _numericSymbolParser;
    private readonly IFxRates _fxRates;

    private ValueTask<ILookup<ulong, Transaction>> GetTransactionsAsync(CancellationToken ct)
        => _client
            .GetLast90DaysTransactionsAsync(ct)
            .Select(transaction =>
            {
                string? variableSymbolRaw = transaction
                    .EntryDetails
                    .TransactionDetails
                    .RemittanceInformation
                    .CreditorReferenceInformation?
                    .Variable;

                ulong? variableSymbol = string.IsNullOrWhiteSpace(variableSymbolRaw)
                    ? null
                    : _numericSymbolParser.Parse(variableSymbolRaw);

                return (variableSymbol, transaction);
            })
            .Where(tuple =>
            {
                if (tuple.variableSymbol is null)
                {
                    _logger.LogInformation("Skipping {ID} due to missing variable symbol.", tuple.transaction.EntryReference);
                    return false;
                }

                if (tuple.transaction is not { BookingDate: { } })
                {
                    _logger.LogInformation("Skipping {ID} due to missing booking date.", tuple.transaction.EntryReference);
                    return false;
                }

                // PERFORMANCE TRADEOFF: prevent needles load from DB by doing the check here thatn in MergePaymentsAsync
                if (GetCounterPartyAccountNumber(tuple.transaction) is null)
                {
                    _logger.LogInformation("Skipping {ID} due to missing account number.", tuple.transaction.EntryReference);
                    return false;
                }

                return true;
            })
            .ToLookupAsync(
                tuple => tuple.variableSymbol!.Value,
                tuple => tuple.transaction);

    private async Task MergePaymentsAsync(
        ICollection<BankPayment> current,
        IEnumerable<Transaction> incoming, 
        CancellationToken ct)
    {
        HashSet<string> alreadyContains = current.Select(p => p.Reference).ToHashSet();

        foreach (Transaction transaction in incoming.Where(t => !alreadyContains.Contains(t.EntryReference)))
        {
            current.Add(new(
                transaction.EntryReference,
                GetCounterPartyAccountNumber(transaction)!,
                GetConstantSymbol(transaction),
                await GetCzkAmountAsync(transaction),
                transaction.BookingDate!.Value,
                transaction
                    .EntryDetails
                    .TransactionDetails
                    .RemittanceInformation
                    .Unstructured,
                new(false, null, null, null)));
        }

        ulong? GetConstantSymbol(Transaction transaction)
        {
            string? constantRaw = transaction
                .EntryDetails
                .TransactionDetails
                .RemittanceInformation
                .CreditorReferenceInformation
                .Constant;

            ulong? constant = string.IsNullOrWhiteSpace(constantRaw)
                ? null
                : _numericSymbolParser.Parse(constantRaw);

            return constant;
        }

        Task<decimal> GetCzkAmountAsync(Transaction transaction)
        {
            decimal value = (decimal)transaction.Amount.Value;
            string currency = transaction.Amount.Currency;

            if (transaction.Amount.Currency.ToLower() is "czk")
                return Task.FromResult(value);

            return _fxRates.ToCzkAsync(value, currency, ct);
        }
    }

    private string? GetCounterPartyAccountNumber(Transaction transaction)
    {
        CounterParty counterParty = transaction.EntryDetails.TransactionDetails.RelatedParties.CounterParty;

        if (counterParty is { Account: { Iban: { } iban } })
            return iban;

        if (counterParty is
            {
                OrganisationIdentification:
                {
                    BankCode: { } bankCode1
                },
                Account:
                {
                    AccountNumber: { } accountNumber1,
                    AccountNumberPrefix: { } prefix1
                }
            })
            return $"{prefix1}-{accountNumber1}/{bankCode1}";

        if (counterParty is
            {
                OrganisationIdentification:
                {
                    BankCode: { } bankCode2
                },
                Account:
                {
                    AccountNumber: { } accountNumber2
                }
            })
            return $"{accountNumber2}/{bankCode2}";

        return null;
    }
}