namespace DSDD.Automations.Payments.Model;

public class Amount
{
    public decimal Value { get; }

    public string Currency { get; }

    public Amount(decimal value, string currency)
    {
        Value = value;
        Currency = currency;
    }
}