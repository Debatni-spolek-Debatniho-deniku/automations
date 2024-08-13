using System.ComponentModel.DataAnnotations;

namespace DSDD.Automations.Payments.Views.Payer;

public class PaymentViewModel
{
    public string Reference { get; }

    [Display(Name = "Typ")]
    public PaymentType Type { get; }

    public ulong VariableSymbol { get; }

    [Display(Name = "Protiúčet")]
    public string? CounterpartyAccountNumber { get; }

    [Display(Name = "Konstantní symbol")]
    public ulong? ConstantSymbol { get; }

    [Display(Name = "Částka")]
    public decimal AmountCzk { get; }

    [Display(Name = "Datum a čas")]
    public DateTime DateTime { get; }

    [Display(Name = "Popis")]
    public string? Description { get; }
    
    public PaymentViewModel(ulong variableSymbol, Model.BankPayment payment)
    {
        Reference = payment.Reference;
        VariableSymbol = variableSymbol;
        CounterpartyAccountNumber = payment.CounterpartyAccountNumber;
        ConstantSymbol = payment.Overrides.ConstantSymbol ?? payment.ConstantSymbol;
        AmountCzk = payment.AmountCzk;
        DateTime =  payment.Overrides.DateTime ?? payment.DateTime;
        Description = payment.Overrides.Description ?? payment.Description;

        Type = payment switch
        {
            { Overrides: { Removed: true } } => PaymentType.BANK_REMOVED,
            { Overriden: true } => PaymentType.BANK_OVERRIDED,
            _ => PaymentType.BANK,
        };
    }

    public PaymentViewModel(ulong variableSymbol, Model.ManualPayment payment)
    {
        Reference = payment.Reference;
        VariableSymbol = variableSymbol;
        CounterpartyAccountNumber = null;
        ConstantSymbol = payment.ConstantSymbol;
        AmountCzk = payment.AmountCzk;
        DateTime = payment.DateTime;
        Description = payment.Description;

        Type = PaymentType.MANUAL;
    }
}