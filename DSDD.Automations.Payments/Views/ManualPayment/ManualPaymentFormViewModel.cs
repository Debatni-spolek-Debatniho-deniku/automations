using System.ComponentModel.DataAnnotations;

namespace DSDD.Automations.Payments.Views.ManualPayment;

public class ManualPaymentFormViewModel
{
    [Required]
    public string? Reference { get; }

    public ulong? ConstantSymbol { get; }

    [Required]
    public decimal? AmountCzk{ get; }

    [Required]
    public DateTime? DateTime { get; }

    public string? Description { get; }

    public ManualPaymentFormViewModel()
    {
    }

    public ManualPaymentFormViewModel(string? reference, ulong? constantSymbol, decimal? amountCzk, DateTime? dateTime, string? description)
    {
        Reference = reference;
        ConstantSymbol = constantSymbol;
        AmountCzk = amountCzk;
        DateTime = dateTime;
        Description = description;
    }

    public ManualPaymentFormViewModel(Model.ManualPayment payment)
        : this(payment.Reference, payment.ConstantSymbol, payment.AmountCzk, payment.DateTime, payment.Description)
    { }
}