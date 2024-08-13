using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using Newtonsoft.Json;

namespace DSDD.Automations.Payments.Model;

public class BankPayment
{
    public string Reference { get; }
    
    public string CounterpartyAccountNumber { get; set; }

    public ulong? ConstantSymbol { get; }

    public decimal AmountCzk { get; }

    public DateTime DateTime { get; }

    public string? Description { get; }

    public BankPaymentOverrides Overrides { get; }

    /// <summary>
    /// Either value from <see cref="Overrides"/> or original bank data.
    /// </summary>
    [JsonIgnore]
    public ulong? FinalConstantSymbol => Overrides.ConstantSymbol ?? ConstantSymbol;

    /// <summary>
    /// Either value from <see cref="Overrides"/> or original bank data.
    /// </summary>
    [JsonIgnore]
    public DateTime FinalDateTime => Overrides.DateTime ?? DateTime;

    /// <summary>
    /// Either value from <see cref="Overrides"/> or original bank data.
    /// </summary>
    [JsonIgnore]
    public string? FinalDescription => Overrides.Description ?? Description;

    /// <summary>
    /// True if any property that overrides bank data in <see cref="Overrides"/> is set. 
    /// </summary>
    [JsonIgnore]
    public bool Overriden => Overrides.ConstantSymbol is not null || Overrides.DateTime is not null || Overrides.Description is not null;

    public BankPayment(string reference, string counterpartyAccountNumber, ulong? constantSymbol, decimal amountCzk, DateTime dateTime, string? description, BankPaymentOverrides overrides)
    {
        Reference = reference;
        CounterpartyAccountNumber = counterpartyAccountNumber;
        ConstantSymbol = constantSymbol;
        AmountCzk = amountCzk;
        DateTime = dateTime;
        Description = description;
        Overrides = overrides;
    }
}