namespace DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;

public class PayerManualPayment
{
    public string Reference { get; }

    public ulong? ConstantSymbol { get; }

    public decimal AmountCzk { get; }

    public DateTime DateTime { get; }

    public string? Description { get; }

    public PayerManualPayment(string reference, ulong? constantSymbol, decimal amountCzk, DateTime dateTime, string? description)
    {
        Reference = reference;
        ConstantSymbol = constantSymbol;
        AmountCzk = amountCzk;
        DateTime = dateTime;
        Description = description;
    }
}