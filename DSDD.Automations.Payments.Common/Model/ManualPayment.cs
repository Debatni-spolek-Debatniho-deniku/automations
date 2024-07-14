namespace DSDD.Automations.Payments.Model;

public class ManualPayment
{
    public string Reference { get; }

    public Amount Amount { get; }

    public string Description { get; }

    public ManualPayment(string reference, Amount amount, string description)
    {
        Reference = reference;
        Amount = amount;
        Description = description;
    }
}