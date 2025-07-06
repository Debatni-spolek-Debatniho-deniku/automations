using Newtonsoft.Json;

namespace DSDD.Automations.Payments.Persistence.Abstractions.Model;

public class UnpairedBankPayment
{
    // For CosmosDB
    [JsonProperty("id")]
    private string Id => Reference;
    
    public string Reference { get; }

    public string CounterpartyAccountNumber { get; set; }

    public ulong? ConstantSymbol { get; }

    public ulong? VariableSymbol { get; }

    public decimal AmountCzk { get; }

    public decimal Amount { get; }

    public string Currency { get; }

    public DateTime DateTime { get; }

    public string? Description { get; }

    public UnpairedBankPayment(string reference, string counterpartyAccountNumber, ulong? constantSymbol, ulong? variableSymbol, decimal amountCzk, decimal amount, string currency, DateTime dateTime, string? description)
    {
        Reference = reference;
        CounterpartyAccountNumber = counterpartyAccountNumber;
        ConstantSymbol = constantSymbol;
        VariableSymbol = variableSymbol;
        AmountCzk = amountCzk;
        Amount = amount;
        Currency = currency;
        DateTime = dateTime;
        Description = description;
    }
}