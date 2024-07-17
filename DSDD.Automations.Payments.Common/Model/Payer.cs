namespace DSDD.Automations.Payments.Model;

public class Payer
{
    public ulong VariableSymbol { get; }

    public ICollection<ManualPayment> ManualPayment { get; } = new List<ManualPayment>();

    public ICollection<BankPayment> BankPayments { get; } = new List<BankPayment>();

    public Payer(ulong variableSymbol)
    {
        VariableSymbol = variableSymbol;
    }
}