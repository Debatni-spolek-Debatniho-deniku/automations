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

    public DateTime DateTime { get; }

    public string? Description { get; }

    public UnpairedBankPayment(string reference, string counterpartyAccountNumber, ulong? constantSymbol, ulong? variableSymbol, decimal amountCzk, DateTime dateTime, string? description)
    {
        Reference = reference;
        CounterpartyAccountNumber = counterpartyAccountNumber;
        ConstantSymbol = constantSymbol;
        VariableSymbol = variableSymbol;
        AmountCzk = amountCzk;
        DateTime = dateTime;
        Description = description;
    }
}