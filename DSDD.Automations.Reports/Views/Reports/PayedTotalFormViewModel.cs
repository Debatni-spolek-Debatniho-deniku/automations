namespace DSDD.Automations.Reports.Views.Reports;

public class PayedTotalFormViewModel
{
    public ulong ConstantSymbol { get; }

    public PayedTotalFormViewModel(ulong constantSymbol)
    {
        ConstantSymbol = constantSymbol;
    }
}