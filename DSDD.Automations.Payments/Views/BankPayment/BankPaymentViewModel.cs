using DSDD.Automations.Payments.Views.Shared;

namespace DSDD.Automations.Payments.Views.BankPayment;

public class BankPaymentViewModel : LayoutViewModel
{
    public string CounterpartyAccountNumber { get; set; }

    public ulong VariableSymbol { get; }

    public ulong? ConstantSymbol { get; }

    public decimal AmountCzk { get; }

    public DateTime DateTime { get; }

    public string? Description { get; }

    public BankPaymentFormViewModel Payment { get; }

    public BankPaymentViewModel(ulong variableSymbol, Model.BankPayment payment) : base($"Bankovní platba poplatníka {variableSymbol}")
    {
        VariableSymbol = variableSymbol;
        CounterpartyAccountNumber = payment.CounterpartyAccountNumber;
        ConstantSymbol = payment.ConstantSymbol;
        AmountCzk = payment.AmountCzk;
        DateTime = payment.DateTime;
        Description = payment.Description;
        Payment = new(payment.Overrides);
    }
}