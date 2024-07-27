namespace DSDD.Automations.Payments.Model;

public class ManualPayment
{
    public string Reference { get; }

    public ulong? ConstantSymbol { get; }

    public decimal AmountCzk { get; }

    public DateTime DateTime { get; }

    public string? Description { get; }

    public ManualPayment(string reference, ulong? constantSymbol, decimal amountCzk, DateTime dateTime, string? description)
    {
        Reference = reference;
        ConstantSymbol = constantSymbol;
        AmountCzk = amountCzk;
        DateTime = dateTime;
        Description = description;
    }
}