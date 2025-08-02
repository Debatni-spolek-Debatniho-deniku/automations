using DSDD.Automations.Payments.Banking.Abstractions;
using DSDD.Automations.Payments.Banking.RBCZ.PremiumApi;
using DSDD.Automations.Payments.Helpers;
using DSDD.Automations.Payments.Persistence.Abstractions;
using DSDD.Automations.Payments.Persistence.Abstractions.Model;
using DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;
using DSDD.Automations.Payments.RBCZ.PremiumApi;
using Microsoft.Extensions.Logging;

namespace DSDD.Automations.Payments.Banking.RBCZ;

internal class BankPaymentsImporter: IBankPaymentsImporter
{
    public BankPaymentsImporter(ILogger<BankPaymentsImporter> logger, IPayersDao payers,
        IPremiumApiClient client, INumericSymbolParser numericSymbolParser, IFxRates fxRates,
        IUnpairedBankPaymentsDao unpairedBankPayments)
    {
        _logger = logger;
        _payers = payers;
        _client = client;
        _numericSymbolParser = numericSymbolParser;
        _fxRates = fxRates;
        _unpairedBankPayments = unpairedBankPayments;
    }

    public async Task ImportAsync(CancellationToken ct)
    {
        _logger.LogInformation("Importing RBCZ payments.");

        IReadOnlyList<Transaction> transactions = await GetTransactionsAsync(ct);

        IEnumerable<Task> transactionTasks = transactions
            .Select(async transaction =>
            {
                UnpairedBankPayment unpairedPayment = new(
                    transaction.EntryReference,
                    GetCounterPartyAccountNumber(transaction)!,
                    GetConstantSymbol(transaction),
                    GetVariableSymbol(transaction),
                    await GetCzkAmountAsync(transaction, ct),
                    (decimal)transaction.Amount.Value,
                    transaction.Amount.Currency,
                    transaction.BookingDate!.Value,
                    transaction.EntryDetails.TransactionDetails.RemittanceInformation.Unstructured);

                // Duplicates will be overriden, better cost wise than loading DB to filter out duplicates.
                await _unpairedBankPayments.UpsertAync(unpairedPayment, ct);
            });

        IEnumerable<Task> payerTasks = GroupByVariableSymbol(transactions)
            .Select(async group =>
            {
                _logger.LogInformation("Working on variable symbol {VariableSymbol}", group.Key);

                Payer? payer = await _payers.GetAsync(group.Key, ct);
                payer ??= new(group.Key);

                await MergePaymentsAsync(payer.BankPayments, group, ct);

                await _payers.UpsertAync(payer, ct);

                _logger.LogInformation("Finished working on variable symbol {VariableSymbol}", group.Key);
            });

        await Task.WhenAll(payerTasks.Concat(transactionTasks));

        _logger.LogInformation("Finished importing RBCZ payments.");
    }

    private readonly ILogger<BankPaymentsImporter> _logger;
    private readonly IPayersDao _payers;
    private readonly IUnpairedBankPaymentsDao _unpairedBankPayments;
    private readonly IPremiumApiClient _client;
    private readonly INumericSymbolParser _numericSymbolParser;
    private readonly IFxRates _fxRates;

    private async ValueTask<IReadOnlyList<Transaction>> GetTransactionsAsync(CancellationToken ct)
        => await _client
            .GetLast90DaysTransactionsAsync(ct)
            .Where(transaction =>
            {
                if (transaction is not { BookingDate: { } })
                {
                    _logger.LogInformation("Skipping {ID} due to missing booking date.", transaction.EntryReference);
                    return false;
                }
                
                if (GetCounterPartyAccountNumber(transaction) is null)
                {
                    _logger.LogInformation("Skipping {ID} due to missing account number.", transaction.EntryReference);
                    return false;
                }

                return true;
            })
            .ToArrayAsync(ct);

    private ILookup<ulong, Transaction> GroupByVariableSymbol(IEnumerable<Transaction> transactions)
        => transactions
            .Select<Transaction, (ulong? VariableSymbol, Transaction Transaction)>(transaction => (GetVariableSymbol(transaction), transaction))
            .Where(tuple =>
            {
                if (tuple.VariableSymbol is null)
                {
                    _logger.LogInformation("Skipping {ID} from sorting by variable symbol due to missing variable symbol.", tuple.Transaction.EntryReference);
                    return false;
                }

                return true;
            }).ToLookup(
                tuple => tuple.VariableSymbol!.Value,
                tuple => tuple.Transaction);

    private async Task MergePaymentsAsync(
        ICollection<PayerBankPayment> current,
        IEnumerable<Transaction> incoming, 
        CancellationToken ct)
    {
        HashSet<string> alreadyContains = current.Select(p => p.Reference).ToHashSet();

        foreach (Transaction transaction in incoming
                      // Skip payments that have been imported before
                     .Where(t => !alreadyContains.Contains(t.EntryReference)))
        {
            current.Add(new(
                transaction.EntryReference,
                GetCounterPartyAccountNumber(transaction)!,
                GetConstantSymbol(transaction),
                await GetCzkAmountAsync(transaction, ct),
                transaction.BookingDate!.Value,
                GetRemittanceInformation(transaction)?.Unstructured,
                new(false, null, null, null)));
        }
    }

    private ulong? GetVariableSymbol(Transaction transaction)
        => GetNumericSymbol(transaction, info => info.Variable);

    private ulong? GetConstantSymbol(Transaction transaction)
        => GetNumericSymbol(transaction, info => info.Constant);

    private ulong? GetNumericSymbol(Transaction transaction, Func<CreditorReferenceInformation, string?> picker)
    {
        if (GetRemittanceInformation(transaction) is not { CreditorReferenceInformation: {} creditorReference })
            return null;

        string? symbolRaw = picker(creditorReference);
        if (string.IsNullOrWhiteSpace(symbolRaw))
            return null;

        return _numericSymbolParser.Parse(symbolRaw);
    }

    private static RemittanceInformation? GetRemittanceInformation(Transaction transaction)
    {
        return transaction
            .EntryDetails
            .TransactionDetails
            .RemittanceInformation;
    }

    private Task<decimal> GetCzkAmountAsync(Transaction transaction, CancellationToken ct)
    {
        decimal value = (decimal)transaction.Amount.Value;
        string currency = transaction.Amount.Currency;

        if (transaction.Amount.Currency.ToLower() is "czk")
            return Task.FromResult(value);

        return _fxRates.ToCzkAsync(value, currency, ct);
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